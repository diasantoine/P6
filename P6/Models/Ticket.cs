namespace P6.Models;

public class Ticket
{
    public int Id { get; set; }
    public int VersionOperatingSystemId { get; set; }
    public int StatusId { get; set; }
    public DateOnly CreatedAt { get; set; }
    public DateOnly? ResolvedAt { get; set; }
    public string Problem { get; set; } = string.Empty;
    public string? Resolution { get; set; }

    public VersionOperatingSystem VersionOperatingSystem { get; set; } = null!;
    public Status Status { get; set; } = null!;
}
