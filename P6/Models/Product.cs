namespace P6.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Version> Versions { get; set; } = new List<Version>();
}
