using CarParkRulesEngine.Models;

namespace CarParkRulesEngine.Services;

public interface IRuleRepository
{
    // Load all rules for a given car park (flattened DTOs)
    Task<List<ParkingRuleDto>> LoadRulesForCarPark(int carParkId);

    // Get the previous exit time for a VRN (for NO_RETURN rule)
    Task<DateTime?> GetPreviousExit(string vrn, int carParkId, DateTime entryTime);

    // Load both rules and sessions for a car park (for the "All Results" view)
    Task<CarParkProcessingData> LoadRulesAndSessions(int carParkId);
}