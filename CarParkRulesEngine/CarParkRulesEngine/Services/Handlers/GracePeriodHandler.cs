using CarParkRulesEngine.Models;

namespace CarParkRulesEngine.Services.Handlers;

public class GracePeriodHandler : IRuleHandler
{
    public string RuleType => "GRACE_PERIOD";

    public RuleHandlerResult Apply(SessionContext ctx, ParkingRuleDto rule, RuleEvaluationResult result)
    {
        if (rule.GraceMinutes == null)
        {
            return new RuleHandlerResult
            {
                RuleApplied = false,
                StopProcessing = false,
                TraceMessage = "GRACE_PERIOD: No grace minutes. Skipped."
            };
        }

        // Calculate the grace cutoff
        var graceCutoff = ctx.EntryTime.AddMinutes(ctx.AllowedMinutes + rule.GraceMinutes.Value);

        // If exit is AFTER grace cutoff → grace does NOT apply
        if (ctx.ExitTime > graceCutoff)
        {
            return new RuleHandlerResult
            {
                RuleApplied = false,
                StopProcessing = false,
                TraceMessage = $"GRACE_PERIOD: Exit beyond grace window. Cutoff={graceCutoff:t}."
            };
        }

        // Otherwise apply grace
        var before = ctx.AllowedMinutes;
        ctx.AllowedMinutes += rule.GraceMinutes.Value;
        bool applied = ctx.AllowedMinutes != before;

        return new RuleHandlerResult
        {
            RuleApplied = applied,
            StopProcessing = false,
            TraceMessage = $"GRACE_PERIOD: Added {rule.GraceMinutes} minutes. Allowed={ctx.AllowedMinutes}."
        };
    }
}
