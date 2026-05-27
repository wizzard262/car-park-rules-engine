using CarParkRulesEngine.Models;

namespace CarParkRulesEngine.Services.Handlers;

public class EnforcementHandler : IRuleHandler
{
    public string RuleType => "ENFORCEMENT";

    public RuleHandlerResult Apply(SessionContext ctx, ParkingRuleDto rule, RuleEvaluationResult result)
    {
        var over = ctx.SessionMinutes - ctx.AllowedMinutes;

        if (over > 0)
        {
            result.IsOverstay = true;
            result.OverstayMinutes = over;

            // If enforcement rule defines a penalty amount:
            //result.Charge = rule.PenaltyAmount; // or whatever your rule uses // ste:todo:

            return new RuleHandlerResult
            {
                RuleApplied = true,
                StopProcessing = true,
                TraceMessage =
                    $"ENFORCEMENT: Session={ctx.SessionMinutes} > Allowed={ctx.AllowedMinutes}. " +
                    $"Overstay={over}. Penalty=YES. Stop."
            };
        }

        // No overstay
        result.IsOverstay = false;
        result.OverstayMinutes = 0;
        result.Charge = 0;

        return new RuleHandlerResult
        {
            RuleApplied = true,
            StopProcessing = true,
            TraceMessage =
                $"ENFORCEMENT: Session={ctx.SessionMinutes} <= Allowed={ctx.AllowedMinutes}. " +
                $"No penalty. Stop."
        };
    }
}
