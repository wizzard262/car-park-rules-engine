using CarParkRulesEngine.Models;

namespace CarParkRulesEngine.Services;

public class RuleEngine(IEnumerable<IRuleHandler> handlers)
{
    private readonly Dictionary<string, IRuleHandler> _handlers = handlers.ToDictionary(
            h => h.RuleType,
            h => h,
            StringComparer.OrdinalIgnoreCase
        );

    public RuleEvaluationResult Evaluate(SessionContext context, List<ParkingRuleDto> rules)
    {
        var result = new RuleEvaluationResult();
        var orderedRules = rules.OrderBy(r => r.Priority).ToList();

        // Rules must be processed in priority order
        foreach (var rule in rules.OrderBy(r => r.Priority))
        {
            if (!_handlers.TryGetValue(rule.RuleType, out var handler))
            {
                result.Trace.Add($"SKIP: No handler for RuleType={rule.RuleType}");
                continue;
            }

            var ruleResult = handler.Apply(context, rule, result);

            // Add trace line
            result.Trace.Add(ruleResult.TraceMessage);

            // If this rule applied, record it
            if (ruleResult.RuleApplied && result.AppliedRule == null)
            {
                result.AppliedRule = rule;
            }

            // Stop if rule says so (e.g., Enforcement or Unlimited Free Parking)
            if (ruleResult.StopProcessing)
            {
                break;
            }
        }

        // Final overstay calculation (if not exempt)
        if (!result.IsExempt)
        {
            var over = context.SessionMinutes - context.AllowedMinutes;

            result.OverstayMinutes = over > 0 ? over : 0;
            result.IsOverstay = result.OverstayMinutes > 0;

            // If no overstay, no charge
            if (!result.IsOverstay)
            {
                result.Charge = 0;
            }
        }
        return result;
    }
}
