using CarParkRulesEngine.Models;

namespace CarParkRulesEngine.Services;

public interface IRuleHandler
{
    string RuleType { get; }
    RuleHandlerResult Apply(SessionContext ctx, ParkingRuleDto rule, RuleEvaluationResult result);
}
