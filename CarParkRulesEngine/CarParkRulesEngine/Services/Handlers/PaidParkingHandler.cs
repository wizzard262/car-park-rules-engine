using CarParkRulesEngine.Models;

namespace CarParkRulesEngine.Services.Handlers;

public class PaidParkingHandler : IRuleHandler
{
    public string RuleType => "PAID";

    public RuleHandlerResult Apply(SessionContext ctx, ParkingRuleDto rule, RuleEvaluationResult result)
    {
        if (rule.PaidMinutes == null)
        {
            return new RuleHandlerResult
            {
                RuleApplied = false,
                StopProcessing = false,
                TraceMessage = "PAID: No paid minutes. Skipped."
            };
        }

        // Only apply if the VRN actually paid
        if (!ctx.IsPaid)
        {
            return new RuleHandlerResult
            {
                RuleApplied = false,
                StopProcessing = false,
                TraceMessage = "PAID: VRN not paid. Skipped."
            };
        }

        var before = ctx.AllowedMinutes;

        ctx.AllowedMinutes = Math.Max(ctx.AllowedMinutes, rule.PaidMinutes.Value);

        bool applied = ctx.AllowedMinutes != before;

        return new RuleHandlerResult
        {
            RuleApplied = applied,
            StopProcessing = false,
            TraceMessage = $"PAID: Allowed={ctx.AllowedMinutes} minutes."
        };
    }

}
