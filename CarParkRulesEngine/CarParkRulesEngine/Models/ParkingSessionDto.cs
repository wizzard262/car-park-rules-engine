namespace CarParkRulesEngine.Models;

public class ParkingSessionDto
{
    public int SessionId { get; set; }
    public string VRN { get; set; } = string.Empty;

    public DateTime EntryTimeUtc { get; set; }
    public DateTime ExitTimeUtc { get; set; }

    public int DurationMinutes { get; set; }
    public int ChargeableMinutes { get; set; }
    public decimal CalculatedCharge { get; set; }

    public bool IsOverstay { get; set; }
    public bool IsPaid { get; set; }
    public bool IsExempt { get; set; }
    public string AppliedRuleType { get; set; } = string.Empty;
    public int AppliedRuleId { get; set; }
}