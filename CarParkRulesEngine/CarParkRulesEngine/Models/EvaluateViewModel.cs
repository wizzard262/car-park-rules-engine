namespace CarParkRulesEngine.Models;

public class EvaluateViewModel
{
    public int CarParkId { get; set; }
    public string VRN { get; set; } = string.Empty;
    public DateTime EntryTime { get; set; }
    public DateTime ExitTime { get; set; }
    public int AllowedMinutes { get; set; }
    public int SessionMinutes { get; set; }
    public RuleEvaluationResult? Result { get; set; }
}
