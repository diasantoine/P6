namespace P6.Models;

// Join table between Version and OperatingSystem (many-to-many):
// one row = one supported Version + OS combination.
public class VersionOperatingSystem
{
    public int Id { get; set; }
    public int VersionId { get; set; }
    public int OperatingSystemId { get; set; }

    public Version Version { get; set; } = null!;
    public OperatingSystem OperatingSystem { get; set; } = null!;
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
