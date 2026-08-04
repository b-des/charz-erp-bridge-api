using System.Text.Json.Serialization;

namespace CharzPiexApi.Domain;

public class VehicleDetails
{
    public required string Value { get; set; }
    public required string Label { get; set; }
    //public bool ShowCheckbox { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<VehicleDetails>? Children { get; set; }
}