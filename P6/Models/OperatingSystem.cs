namespace P6.Models;

public class OperatingSystem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<VersionOperatingSystem> VersionOperatingSystems { get; set; } = new List<VersionOperatingSystem>();
}
