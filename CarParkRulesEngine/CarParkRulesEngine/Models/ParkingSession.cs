namespace CarParkRulesEngine.Models;

public class ParkingSession
{
    public int ParkingSessionId { get; set; }
    public int CarParkId { get; set; }
    public string VRN { get; set; } = string.Empty;

    public DateTime EntryTime { get; set; }
    public DateTime ExitTime { get; set; }

    public int CalculatedAllowedMinutes { get; set; }
    public int CalculatedOverstayMinutes { get; set; }
    public decimal CalculatedPenalty { get; set; }

    public bool? IsPaid { get; set; }
    public bool? IsValidated { get; set; }
}
