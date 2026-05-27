namespace CarParkRulesEngine.Services;

public class RuleHandlerResult
{
    public bool StopProcessing { get; set; }
    public bool RuleApplied { get; set; }
    public string TraceMessage { get; set; } = string.Empty;
}
