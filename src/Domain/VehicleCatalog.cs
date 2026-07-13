namespace CharzPiexApi.Domain;

public record VehicleCatalog(
    string Value,
    string Label,
    List<object> Nodes);