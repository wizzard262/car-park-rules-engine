namespace CarParkRulesEngine.Services;

public class RuleResult
{
    // If true, the rule engine stops evaluating further rules
    public bool StopProcessing { get; set; }

    // Human-readable trace message for debugging / UI output
    public string TraceMessage { get; set; } = string.Empty;

    // If true, the rule engine stops evaluating further rules
    public bool RuleApplied { get; set; }
}
