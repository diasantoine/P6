namespace P6.Models;

public class Version
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Number { get; set; } = string.Empty;

    public Product Product { get; set; } = null!;
    public ICollection<VersionOperatingSystem> VersionOperatingSystems { get; set; } = new List<VersionOperatingSystem>();
}
