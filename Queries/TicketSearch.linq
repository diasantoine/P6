<Query Kind="Statements">
  <Connection>
    <Server>localhost</Server>
    <Database>NexaWorks</Database>
    <DriverData>
      <TrustServerCertificate>true</TrustServerCertificate>
    </DriverData>
  </Connection>
</Query>

// Ticket search for NexaWorks.
// One parameterized query covers the 20 requests of the list: in progress / resolved, for a product,
// a version, a period and keywords are all the same query with different criteria.
// Each call below is one request; open the "SQL" tab to see the single SELECT sent to SQL Server.

const int InProgress = 1;
const int Resolved = 2;

// --- In progress tickets (period on the creation date) ---
Search(InProgress)
    .Dump("01 - In progress, all products");
Search(InProgress, product: "Trader en Herbe")
    .Dump("02 - In progress, one product (all versions)");
Search(InProgress, product: "Trader en Herbe", version: "1.2")
    .Dump("03 - In progress, one product (one version)");
Search(InProgress, product: "Trader en Herbe", from: new DateTime(2023, 1, 1), to: new DateTime(2024, 12, 31))
    .Dump("04 - In progress, period, one product (all versions)");
Search(InProgress, product: "Trader en Herbe", version: "1.3", from: new DateTime(2024, 1, 1), to: new DateTime(2024, 12, 31))
    .Dump("05 - In progress, period, one product (one version)");
Search(InProgress, keywords: new[] { "utilisateur", "écran" })
    .Dump("06 - In progress, keywords (all products)");
Search(InProgress, product: "Trader en Herbe", keywords: new[] { "utilisateur", "écran" })
    .Dump("07 - In progress, one product, keywords (all versions)");
Search(InProgress, product: "Trader en Herbe", version: "1.3", keywords: new[] { "euro" })
    .Dump("08 - In progress, one product, keywords (one version)");
Search(InProgress, product: "Trader en Herbe", from: new DateTime(2024, 1, 1), to: new DateTime(2024, 12, 31), keywords: new[] { "écran" })
    .Dump("09 - In progress, period, one product, keywords (all versions)");
Search(InProgress, product: "Trader en Herbe", version: "1.2", from: new DateTime(2023, 1, 1), to: new DateTime(2023, 12, 31), keywords: new[] { "écran" })
    .Dump("10 - In progress, period, one product, keywords (one version)");

// --- Resolved tickets (period on the resolution date) ---
Search(Resolved)
    .Dump("11 - Resolved, all products");
Search(Resolved, product: "Trader en Herbe")
    .Dump("12 - Resolved, one product (all versions)");
Search(Resolved, product: "Trader en Herbe", version: "1.0")
    .Dump("13 - Resolved, one product (one version)");
Search(Resolved, product: "Trader en Herbe", from: new DateTime(2022, 1, 1), to: new DateTime(2022, 12, 31))
    .Dump("14 - Resolved, period, one product (all versions)");
Search(Resolved, product: "Trader en Herbe", version: "1.0", from: new DateTime(2022, 4, 1), to: new DateTime(2022, 6, 30))
    .Dump("15 - Resolved, period, one product (one version)");
Search(Resolved, keywords: new[] { "mot de passe" })
    .Dump("16 - Resolved, keywords (all products)");
Search(Resolved, product: "Maître des Investissements", keywords: new[] { "mot de passe" })
    .Dump("17 - Resolved, one product, keywords (all versions)");
Search(Resolved, product: "Trader en Herbe", version: "1.1", keywords: new[] { "CSV" })
    .Dump("18 - Resolved, one product, keywords (one version)");
Search(Resolved, product: "Maître des Investissements", from: new DateTime(2022, 1, 1), to: new DateTime(2022, 12, 31), keywords: new[] { "relevé", "OFX" })
    .Dump("19 - Resolved, period, one product, keywords (all versions)");
Search(Resolved, product: "Trader en Herbe", version: "1.3", from: new DateTime(2024, 1, 1), to: new DateTime(2024, 12, 31), keywords: new[] { "Face ID" })
    .Dump("20 - Resolved, period, one product, keywords (one version)");

// The only query. Status is required; every other criterion is added only when it is filled.
// Nothing is sent to the database while the query is built: it runs once, when Dump() reads it.
IQueryable<object> Search(
    int status,
    string product = null,
    string version = null,
    DateTime? from = null,  // period start (inclusive)
    DateTime? to = null,    // period end (inclusive)
    string[] keywords = null)  // words that must all appear in the problem description
{
    var query = Tickets.Where(t => t.StatusId == status);

    if (!string.IsNullOrWhiteSpace(product))
    {
        query = query.Where(t => t.VersionOperatingSystem.Version.Product.Name == product);
    }

    if (!string.IsNullOrWhiteSpace(version))
    {
        query = query.Where(t => t.VersionOperatingSystem.Version.Number == version);
    }

    // In progress: period on the date the problem was reported ("rencontrés au cours d'une période").
    // Resolved: period on the date it was resolved ("résolus au cours d'une période").
    if (status == InProgress)
    {
        if (from is not null) query = query.Where(t => t.CreatedAt >= from.Value);
        if (to is not null) query = query.Where(t => t.CreatedAt <= to.Value);
    }
    else
    {
        if (from is not null) query = query.Where(t => t.ResolvedAt >= from.Value);
        if (to is not null) query = query.Where(t => t.ResolvedAt <= to.Value);
    }

    // One Where per keyword: the ticket must contain all of them (translated to LIKE '%word%').
    foreach (var keyword in keywords ?? Array.Empty<string>())
    {
        query = query.Where(t => t.Problem.Contains(keyword));
    }

    // Projection: only the needed columns are read, the joins are generated by LINQ.
    return query
        .OrderBy(t => t.CreatedAt)
        .Select(t => new
        {
            t.Id,
            Product = t.VersionOperatingSystem.Version.Product.Name,
            Version = t.VersionOperatingSystem.Version.Number,
            OperatingSystem = t.VersionOperatingSystem.OperatingSystem.Name,
            Status = t.Status.Label,
            t.CreatedAt,
            t.ResolvedAt,
            t.Problem,
            t.Resolution
        });
}
