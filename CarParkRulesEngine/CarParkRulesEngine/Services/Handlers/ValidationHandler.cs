using CarParkRulesEngine.Models;

namespace CarParkRulesEngine.Services.Handlers;

public class ValidationHandler : IRuleHandler
{
    public string RuleType => "VALIDATION";

    public RuleHandlerResult Apply(SessionContext ctx, ParkingRuleDto rule, RuleEvaluationResult result)
    {
        if (rule.ValidatedMinutes == null)
        {
            return new RuleHandlerResult
            {
                RuleApplied = false,
                StopProcessing = false,
                TraceMessage = "VALIDATION: No validated minutes. Skipped."
            };
        }

        // Only apply if the VRN is validated
        if (!ctx.IsValidated)
        {
            return new RuleHandlerResult
            {
                RuleApplied = false,
                StopProcessing = false,
                TraceMessage = "VALIDATION: VRN not validated. Skipped."
            };
        }

        var before = ctx.AllowedMinutes;

        ctx.AllowedMinutes = Math.Max(ctx.AllowedMinutes, rule.ValidatedMinutes.Value);

        bool applied = ctx.AllowedMinutes != before;

        return new RuleHandlerResult
        {
            RuleApplied = applied,
            StopProcessing = false,
            TraceMessage = $"VALIDATION: Allowed={ctx.AllowedMinutes} minutes."
        };
    }
}
