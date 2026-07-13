namespace CharzPiexApi.Domain;

public class DefectEntity
{
    public int Id { get; set; }
    public required string Model { get; set; }
    public required string Order { get; set; }
    public required string Engine { get; set; }
    public required string Chassis { get; set; }
    public string DocumentRef { get; set; } = string.Empty;
    public DateTime Created { get; init; } = DateTime.Now;
    public DateTime Modified { get; init; } = DateTime.Now;
}