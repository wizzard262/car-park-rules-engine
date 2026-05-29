namespace CarParkRulesEngine.Models;

public class ParkingRuleDto
{
    public int ParkingRuleId { get; set; }
    public int CarParkId { get; set; }

    // Rule header
    public string RuleType { get; set; } = string.Empty;
    public int Priority { get; set; }
    public bool AppliesToAllVRNs { get; set; }
    public int? VRNGroupId { get; set; }
    public int? TimeWindowId { get; set; }

    // Parameter tables (flattened)
    public int? MaxFreeMinutes { get; set; }       // Rule_FreeParking
    public int? NoReturnMinutes { get; set; }      // Rule_NoReturn
    public int? GraceMinutes { get; set; }         // Rule_GracePeriod
    public int? PaidMinutes { get; set; }          // Rule_PaidParking
    public int? ValidatedMinutes { get; set; }     // Rule_Validation

    // Time window (flattened)
    public TimeSpan? WindowStart { get; set; }
    public TimeSpan? WindowEnd { get; set; }
    public int? DaysOfWeekMask { get; set; }
}
