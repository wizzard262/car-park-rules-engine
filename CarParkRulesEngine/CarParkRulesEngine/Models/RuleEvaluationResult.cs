namespace CarParkRulesEngine.Models;

public class RuleEvaluationResult
{
    public decimal Charge { get; set; }
    public bool IsOverstay { get; set; }
    public bool IsExempt { get; set; }
    public int OverstayMinutes { get; set; }

    // The rule that actually fired
    public ParkingRuleDto? AppliedRule { get; set; }

    // Optional: full trace of rule evaluation
    public List<string> Trace { get; set; } = [];
}