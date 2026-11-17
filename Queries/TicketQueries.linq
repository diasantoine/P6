<Query Kind="Statements">
  <Connection>
    <Server>localhost</Server>
    <Database>NexaWorks</Database>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DriverData>
      <TrustServerCertificate>true</TrustServerCertificate>
    </DriverData>
  </Connection>
</Query>

// The 20 requests of the list, one query each, numbered as in the list.
// Status: 1 = "En cours", 2 = "Résolu".
// Period: on CreatedAt for in-progress tickets ("rencontrés au cours d'une période"),
//         on ResolvedAt for resolved tickets ("résolus au cours d'une période"), bounds included.
// Keywords: one Contains per word, all required (translated to LIKE '%word%').
// Dates: CreatedAt and ResolvedAt are SQL "date" columns, DateOnly in the project.
//        The connection header enables "Allow DateOnly/TimeOnly" (LINQPad default), so the
//        columns are DateOnly here too. Keep that option checked if you create the connection by hand.

// ----- Problèmes en cours -----

// 01
Tickets.Where(t => t.StatusId == 1)
    .Dump("01 - Tous les problèmes en cours (tous les produits)");

// 02
Tickets.Where(t => t.StatusId == 1
              && t.VersionOperatingSystem.Version.Product.Name == "Trader en Herbe")
    .Dump("02 - Problèmes en cours d'un produit (toutes versions)");

// 03
Tickets.Where(t => t.StatusId == 1
              && t.VersionOperatingSystem.Version.Product.Name == "Trader en Herbe"
              && t.VersionOperatingSystem.Version.Number == "1.2")
    .Dump("03 - Problèmes en cours d'un produit (une version)");

// 04
Tickets.Where(t => t.StatusId == 1
              && t.VersionOperatingSystem.Version.Product.Name == "Trader en Herbe"
              && t.CreatedAt >= new DateOnly(2023, 1, 1)
              && t.CreatedAt <= new DateOnly(2024, 12, 31))
    .Dump("04 - Problèmes en cours rencontrés au cours d'une période, d'un produit (toutes versions)");

// 05
Tickets.Where(t => t.StatusId == 1
              && t.VersionOperatingSystem.Version.Product.Name == "Trader en Herbe"
              && t.VersionOperatingSystem.Version.Number == "1.3"
              && t.CreatedAt >= new DateOnly(2024, 1, 1)
              && t.CreatedAt <= new DateOnly(2024, 12, 31))
    .Dump("05 - Problèmes en cours rencontrés au cours d'une période, d'un produit (une version)");

// 06
Tickets.Where(t => t.StatusId == 1
              && t.Problem.Contains("utilisateur")
              && t.Problem.Contains("écran"))
    .Dump("06 - Problèmes en cours contenant des mots-clés (tous les produits)");

// 07
Tickets.Where(t => t.StatusId == 1
              && t.VersionOperatingSystem.Version.Product.Name == "Trader en Herbe"
              && t.Problem.Contains("utilisateur")
              && t.Problem.Contains("écran"))
    .Dump("07 - Problèmes en cours contenant des mots-clés, d'un produit (toutes versions)");

// 08
Tickets.Where(t => t.StatusId == 1
              && t.VersionOperatingSystem.Version.Product.Name == "Trader en Herbe"
              && t.VersionOperatingSystem.Version.Number == "1.3"
              && t.Problem.Contains("euro"))
    .Dump("08 - Problèmes en cours contenant des mots-clés, d'un produit (une version)");

// 09
Tickets.Where(t => t.StatusId == 1
              && t.VersionOperatingSystem.Version.Product.Name == "Trader en Herbe"
              && t.CreatedAt >= new DateOnly(2024, 1, 1)
              && t.CreatedAt <= new DateOnly(2024, 12, 31)
              && t.Problem.Contains("écran"))
    .Dump("09 - Problèmes en cours rencontrés au cours d'une période, contenant des mots-clés, d'un produit (toutes versions)");

// 10
Tickets.Where(t => t.StatusId == 1
              && t.VersionOperatingSystem.Version.Product.Name == "Trader en Herbe"
              && t.VersionOperatingSystem.Version.Number == "1.2"
              && t.CreatedAt >= new DateOnly(2023, 1, 1)
              && t.CreatedAt <= new DateOnly(2023, 12, 31)
              && t.Problem.Contains("écran"))
    .Dump("10 - Problèmes en cours rencontrés au cours d'une période, contenant des mots-clés, d'un produit (une version)");

// ----- Problèmes résolus -----

// 11
Tickets.Where(t => t.StatusId == 2)
    .Dump("11 - Tous les problèmes résolus (tous les produits)");

// 12
Tickets.Where(t => t.StatusId == 2
              && t.VersionOperatingSystem.Version.Product.Name == "Trader en Herbe")
    .Dump("12 - Problèmes résolus d'un produit (toutes versions)");

// 13
Tickets.Where(t => t.StatusId == 2
              && t.VersionOperatingSystem.Version.Product.Name == "Trader en Herbe"
              && t.VersionOperatingSystem.Version.Number == "1.0")
    .Dump("13 - Problèmes résolus d'un produit (une version)");

// 14
Tickets.Where(t => t.StatusId == 2
              && t.VersionOperatingSystem.Version.Product.Name == "Trader en Herbe"
              && t.ResolvedAt >= new DateOnly(2022, 1, 1)
              && t.ResolvedAt <= new DateOnly(2022, 12, 31))
    .Dump("14 - Problèmes résolus au cours d'une période, d'un produit (toutes versions)");

// 15
Tickets.Where(t => t.StatusId == 2
              && t.VersionOperatingSystem.Version.Product.Name == "Trader en Herbe"
              && t.VersionOperatingSystem.Version.Number == "1.0"
              && t.ResolvedAt >= new DateOnly(2022, 4, 1)
              && t.ResolvedAt <= new DateOnly(2022, 6, 30))
    .Dump("15 - Problèmes résolus au cours d'une période, d'un produit (une version)");

// 16
Tickets.Where(t => t.StatusId == 2
              && t.Problem.Contains("mot de passe"))
    .Dump("16 - Problèmes résolus contenant des mots-clés (tous les produits)");

// 17
Tickets.Where(t => t.StatusId == 2
              && t.VersionOperatingSystem.Version.Product.Name == "Maître des Investissements"
              && t.Problem.Contains("mot de passe"))
    .Dump("17 - Problèmes résolus contenant des mots-clés, d'un produit (toutes versions)");

// 18
Tickets.Where(t => t.StatusId == 2
              && t.VersionOperatingSystem.Version.Product.Name == "Trader en Herbe"
              && t.VersionOperatingSystem.Version.Number == "1.1"
              && t.Problem.Contains("CSV"))
    .Dump("18 - Problèmes résolus contenant des mots-clés, d'un produit (une version)");

// 19
Tickets.Where(t => t.StatusId == 2
              && t.VersionOperatingSystem.Version.Product.Name == "Maître des Investissements"
              && t.ResolvedAt >= new DateOnly(2022, 1, 1)
              && t.ResolvedAt <= new DateOnly(2022, 12, 31)
              && t.Problem.Contains("relevé")
              && t.Problem.Contains("OFX"))
    .Dump("19 - Problèmes résolus au cours d'une période, contenant des mots-clés, d'un produit (toutes versions)");

// 20
Tickets.Where(t => t.StatusId == 2
              && t.VersionOperatingSystem.Version.Product.Name == "Trader en Herbe"
              && t.VersionOperatingSystem.Version.Number == "1.3"
              && t.ResolvedAt >= new DateOnly(2024, 1, 1)
              && t.ResolvedAt <= new DateOnly(2024, 12, 31)
              && t.Problem.Contains("Face ID"))
    .Dump("20 - Problèmes résolus au cours d'une période, contenant des mots-clés, d'un produit (une version)");
