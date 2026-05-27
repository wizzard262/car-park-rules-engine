namespace CarParkRulesEngine.Models;

public class CarParkProcessingData
{
    public List<ParkingRuleDto> Rules { get; set; } = [];
    public List<ParkingSessionDto> Sessions { get; set; } = [];
}