using CarParkRulesEngine.Models;
using Dapper;
using System.Data.SqlClient;

namespace CarParkRulesEngine.Services;

public class SqlRuleRepository(IConfiguration config, RuleEngine engine) : IRuleRepository
{
    private readonly IConfiguration _config = config;
    private readonly RuleEngine _engine = engine;

    public async Task<List<ParkingRuleDto>> LoadRulesForCarPark(int carParkId)
    {
        using var conn = new SqlConnection(_config.GetConnectionString("Default"));

        var sql = @"
            SELECT r.*, 
                   fp.MaxFreeMinutes,
                   nr.NoReturnMinutes,
                   gp.GraceMinutes,
                   pp.PaidMinutes,
                   v.ValidatedMinutes,
                   tw.StartTime AS WindowStart,
                   tw.EndTime AS WindowEnd,
                   tw.DaysOfWeekMask
            FROM ParkingRule r
            LEFT JOIN Rule_FreeParking fp ON fp.ParkingRuleId = r.ParkingRuleId
            LEFT JOIN Rule_NoReturn nr ON nr.ParkingRuleId = r.ParkingRuleId
            LEFT JOIN Rule_GracePeriod gp ON gp.ParkingRuleId = r.ParkingRuleId
            LEFT JOIN Rule_PaidParking pp ON pp.ParkingRuleId = r.ParkingRuleId
            LEFT JOIN Rule_Validation v ON v.ParkingRuleId = r.ParkingRuleId
            LEFT JOIN TimeWindow tw ON tw.TimeWindowId = r.TimeWindowId
            WHERE r.CarParkId = @carParkId
            ORDER BY r.Priority;
            ";

        return (await conn.QueryAsync<ParkingRuleDto>(sql, new { carParkId })).ToList();
    }

    public async Task<DateTime?> GetPreviousExit(string vrn, int carParkId, DateTime entryTime)
    {
        using var conn = new SqlConnection(_config.GetConnectionString("Default"));

        var sql = @"
        SELECT TOP 1 ExitTime
        FROM ParkingSession
        WHERE VRN = @vrn
          AND CarParkId = @carParkId
          AND ExitTime < @entryTime
        ORDER BY ExitTime DESC;";

        return await conn.QueryFirstOrDefaultAsync<DateTime?>(sql, new { vrn, carParkId, entryTime });
    }


    public async Task<CarParkProcessingData> LoadRulesAndSessions(int carParkId)
    {
        using var conn = new SqlConnection(_config.GetConnectionString("Default"));

        var result = new CarParkProcessingData();

        // 1. Load rules (reuse your existing SQL)
        var rulesSql = @"
        SELECT r.*, 
               fp.MaxFreeMinutes,
               nr.NoReturnMinutes,
               gp.GraceMinutes,
               pp.PaidMinutes,
               v.ValidatedMinutes,
               tw.StartTime AS WindowStart,
               tw.EndTime AS WindowEnd,
               tw.DaysOfWeekMask
        FROM ParkingRule r
        LEFT JOIN Rule_FreeParking fp ON fp.ParkingRuleId = r.ParkingRuleId
        LEFT JOIN Rule_NoReturn nr ON nr.ParkingRuleId = r.ParkingRuleId
        LEFT JOIN Rule_GracePeriod gp ON gp.ParkingRuleId = r.ParkingRuleId
        LEFT JOIN Rule_PaidParking pp ON pp.ParkingRuleId = r.ParkingRuleId
        LEFT JOIN Rule_Validation v ON v.ParkingRuleId = r.ParkingRuleId
        LEFT JOIN TimeWindow tw ON tw.TimeWindowId = r.TimeWindowId
        WHERE r.CarParkId = @carParkId
        ORDER BY r.Priority;";

        var rules = await conn.QueryAsync<ParkingRuleDto>(rulesSql, new { carParkId });
        result.Rules = [.. rules];

        // 2. Load sessions
        var sessionsSql = @"
        SELECT ParkingSessionId, CarParkId, VRN, EntryTime, ExitTime, CalculatedAllowedMinutes, CalculatedOverstayMinutes, CalculatedPenalty, IsPaid, IsValidated
        FROM ParkingSession
        WHERE CarParkId = @carParkId
        ORDER BY EntryTime DESC;";

        var sessions = await conn.QueryAsync<ParkingSession>(sessionsSql, new { carParkId });

        var output = new List<ParkingSessionDto>();

        foreach (var s in sessions)
        {
            // 1. Convert DB row → SessionContext
            var ctx = new SessionContext
            {
                VRN = s.VRN,
                CarParkId = s.CarParkId,
                EntryTime = s.EntryTime,
                ExitTime = s.ExitTime,
                PreviousExitTime = await GetPreviousExit(s.VRN, s.CarParkId, s.EntryTime),
                IsPaid = s.IsPaid == true,
                IsValidated = s.IsValidated == true
            };

            // 2. Run the rule engine
            var eval = _engine.Evaluate(ctx, [.. rules]);

            // 3. Convert to UI DTO
            var dto = MapToResult(s, eval);

            output.Add(dto);
        }

        result.Sessions = output;
        return result;
    }

    private ParkingSessionDto MapToResult(
    ParkingSession s,
    RuleEvaluationResult eval)
    {
        return new ParkingSessionDto
        {
            SessionId = s.ParkingSessionId,
            VRN = s.VRN,

            EntryTimeUtc = s.EntryTime,
            ExitTimeUtc = s.ExitTime,

            DurationMinutes = (int)(s.ExitTime - s.EntryTime).TotalMinutes,
            ChargeableMinutes = eval.IsOverstay ? eval.OverstayMinutes : 0,
            CalculatedCharge = eval.Charge,

            IsOverstay = eval.IsOverstay,
            IsPaid = eval.Charge == 0,
            IsExempt = eval.IsExempt,

            AppliedRuleId = eval.AppliedRule?.ParkingRuleId ?? 0,
            AppliedRuleType = eval.AppliedRule?.RuleType ?? "None"
        };
    }
}