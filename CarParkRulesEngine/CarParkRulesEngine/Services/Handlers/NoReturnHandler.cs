using CarParkRulesEngine.Models;

namespace CarParkRulesEngine.Services.Handlers;

public class NoReturnHandler : IRuleHandler
{
    public string RuleType => "NO_RETURN";
    public RuleHandlerResult Apply(SessionContext ctx, ParkingRuleDto rule, RuleEvaluationResult result)
    {
        // No previous session
        if (ctx.PreviousExitTime == null || rule.NoReturnMinutes == null)
        {
            return new RuleHandlerResult
            {
                RuleApplied = false,
                StopProcessing = false,
                TraceMessage = "NO_RETURN: No previous session or no rule minutes. Skipped."
            };
        }

        var minutesSinceLastExit = (int)(ctx.EntryTime - ctx.PreviousExitTime.Value).TotalMinutes;

        // violation if re-entry is within the NoReturnMinutes window
        if (minutesSinceLastExit < rule.NoReturnMinutes.Value)
        {
            // This is a violation → instant penalty
            result.IsOverstay = true;          // treat as penalty
            result.OverstayMinutes = 0;        // no overstay calculation
            // result.Charge = rule.PenaltyAmount ?? 0; // if your rule has a penalty amount //ste:todo:

            return new RuleHandlerResult
            {
                RuleApplied = false, // the rule didn’t apply, it just passed.
                StopProcessing = false,
                TraceMessage = $"NO_RETURN: Violation. Re-entry after {minutesSinceLastExit} mins < {rule.NoReturnMinutes}. Penalty=YES. Stop."
            };
        }

        return new RuleHandlerResult
        {
            RuleApplied = true,
            StopProcessing = false,
            TraceMessage =
                $"NO_RETURN: OK. Re-entry after {minutesSinceLastExit} mins >= {rule.NoReturnMinutes} mins."
        };
    }
}
