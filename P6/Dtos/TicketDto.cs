namespace P6.Dtos;

/// <summary>A ticket as returned by the queries: only the columns needed for display.</summary>
public record TicketDto(
    int Id,
    string Product,
    string Version,
    string OperatingSystem,
    string Status,
    DateOnly CreatedAt,
    DateOnly? ResolvedAt,
    string Problem,
    string? Resolution);
