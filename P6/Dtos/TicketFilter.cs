namespace P6.Dtos;

/// <summary>
/// Search criteria for tickets. Only the status is required; every other criterion
/// is applied only when it is filled.
/// </summary>
public class TicketFilter
{
    /// <summary>1 = in progress, 2 = resolved (see SeedData).</summary>
    public int Status { get; set; }

    /// <summary>Product name, e.g. "Trader en Herbe".</summary>
    public string? Product { get; set; }

    /// <summary>Version number, e.g. "1.2". Meant to be used with <see cref="Product"/>.</summary>
    public string? Version { get; set; }

    /// <summary>Period start (inclusive), on the creation date.</summary>
    public DateOnly? From { get; set; }

    /// <summary>Period end (inclusive), on the creation date.</summary>
    public DateOnly? To { get; set; }

    /// <summary>Words that must all appear in the problem description.</summary>
    public string[]? Keywords { get; set; }
}
