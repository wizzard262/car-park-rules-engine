using System.ComponentModel.DataAnnotations;

namespace CarParkRulesEngine.Models;

public class EvaluateViewModel
{
    public int CarParkId { get; set; } = 1;

    [Required]
    public string VRN { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime EntryTime { get; set; } = DateTime.Now;

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime ExitTime { get; set; } = DateTime.Now;
    public int AllowedMinutes { get; set; }
    public int SessionMinutes { get; set; }
    public RuleEvaluationResult? Result { get; set; }
}
