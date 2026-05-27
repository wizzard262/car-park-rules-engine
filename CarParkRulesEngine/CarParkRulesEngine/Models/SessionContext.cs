namespace CarParkRulesEngine.Models;

public class SessionContext
{
    public int CarParkId { get; set; }
    public string VRN { get; set; } = string.Empty;

    public DateTime EntryTime { get; set; }
    public DateTime ExitTime { get; set; }

    // For No-Return rule
    public DateTime? PreviousExitTime { get; set; }
    public bool IsValidated { get; set; }
    public bool IsPaid { get; set; }

    public int AllowedMinutes { get; set; } = 0;

    // Computed duration of the session
    public int SessionMinutes => (int)(ExitTime - EntryTime).TotalMinutes;
}
