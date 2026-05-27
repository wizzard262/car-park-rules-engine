using CarParkRulesEngine.Models;

namespace CarParkRulesEngine.Services.Handlers;

public class FreeParkingHandler : IRuleHandler
{
    public string RuleType => "FREE_PARKING";

    public RuleHandlerResult Apply(SessionContext ctx, ParkingRuleDto rule, RuleEvaluationResult result)
    {
        if (!IsInWindow(ctx, rule))
        {
            return new RuleHandlerResult
            {
                RuleApplied = false,
                StopProcessing = false,
                TraceMessage = "FREE_PARKING: Not in time window."
            };
        }

        // Unlimited free parking
        if (rule.MaxFreeMinutes == null)
        {
            ctx.AllowedMinutes = int.MaxValue;
            return new RuleHandlerResult
            {
                RuleApplied = true,
                StopProcessing = true,
                TraceMessage = "FREE_PARKING: Unlimited free parking. Allowed = ∞. Stop."
            };
        }

        // Track before/after to detect whether rule actually applied
        var before = ctx.AllowedMinutes;

        ctx.AllowedMinutes = Math.Max(ctx.AllowedMinutes, rule.MaxFreeMinutes.Value);

        bool applied = ctx.AllowedMinutes != before;

        return new RuleHandlerResult
        {
            RuleApplied = applied,
            StopProcessing = false,
            TraceMessage = $"FREE_PARKING: MaxFree={rule.MaxFreeMinutes} minutes. Allowed={ctx.AllowedMinutes}."
        };
    }


    private static bool IsInWindow(SessionContext ctx, ParkingRuleDto rule)
    {
        if (rule.WindowStart == null || rule.WindowEnd == null)
            return true;

        var t = ctx.EntryTime.TimeOfDay;
        var start = rule.WindowStart.Value;
        var end = rule.WindowEnd.Value;

        if (start <= end)
            return t >= start && t <= end;

        // Overnight window (e.g. 18:00 → 08:00)
        return t >= start || t <= end;
    }
}
