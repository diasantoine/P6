namespace P6.Models;

public class Status
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
