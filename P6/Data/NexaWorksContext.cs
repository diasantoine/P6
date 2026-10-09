using Microsoft.EntityFrameworkCore;
using P6.Models;
// Our entities Version and OperatingSystem share their names with System.Version
// and System.OperatingSystem: these aliases make the compiler pick ours.
using OperatingSystem = P6.Models.OperatingSystem;
using Version = P6.Models.Version;

namespace P6.Data;

public class NexaWorksContext : DbContext
{
    public NexaWorksContext(DbContextOptions<NexaWorksContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Version> Versions => Set<Version>();
    public DbSet<OperatingSystem> OperatingSystems => Set<OperatingSystem>();
    public DbSet<VersionOperatingSystem> VersionOperatingSystems => Set<VersionOperatingSystem>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Status> Statuses => Set<Status>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");
            entity.Property(p => p.Name).HasMaxLength(100).IsRequired();
            entity.HasIndex(p => p.Name).IsUnique();
        });

        modelBuilder.Entity<Version>(entity =>
        {
            entity.ToTable("Version");
            entity.Property(v => v.Number).HasMaxLength(10).IsRequired();
            // "1.0" exists for several products: only the (product, number) pair is unique.
            entity.HasIndex(v => new { v.ProductId, v.Number }).IsUnique();

            // Product 1 -- 1..* Version
            entity.HasOne(v => v.Product)
                  .WithMany(p => p.Versions)
                  .HasForeignKey(v => v.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OperatingSystem>(entity =>
        {
            entity.ToTable("OperatingSystem");
            entity.Property(os => os.Name).HasMaxLength(50).IsRequired();
            entity.HasIndex(os => os.Name).IsUnique();
        });

        modelBuilder.Entity<VersionOperatingSystem>(entity =>
        {
            entity.ToTable("VersionOperatingSystem");
            // A compatibility can only be declared once.
            entity.HasIndex(vos => new { vos.VersionId, vos.OperatingSystemId }).IsUnique();

            // Version 1 -- 1..* VersionOperatingSystem
            entity.HasOne(vos => vos.Version)
                  .WithMany(v => v.VersionOperatingSystems)
                  .HasForeignKey(vos => vos.VersionId)
                  .OnDelete(DeleteBehavior.Restrict);

            // OperatingSystem 1 -- 0..* VersionOperatingSystem
            entity.HasOne(vos => vos.OperatingSystem)
                  .WithMany(os => os.VersionOperatingSystems)
                  .HasForeignKey(vos => vos.OperatingSystemId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.ToTable("Status");
            entity.Property(s => s.Label).HasMaxLength(20).IsRequired();
            entity.HasIndex(s => s.Label).IsUnique();
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("Ticket", table =>
            {
                table.HasCheckConstraint("CK_Ticket_ResolvedAt_AfterCreatedAt",
                    "[ResolvedAt] IS NULL OR [ResolvedAt] >= [CreatedAt]");
                // "In progress" => no resolution yet; "Resolved" => resolution date and text filled.
                table.HasCheckConstraint("CK_Ticket_Status_Resolution",
                    $"([StatusId] = {SeedData.StatusInProgressId} AND [ResolvedAt] IS NULL AND [Resolution] IS NULL) " +
                    $"OR ([StatusId] = {SeedData.StatusResolvedId} AND [ResolvedAt] IS NOT NULL AND [Resolution] IS NOT NULL)");
            });
            entity.Property(t => t.Problem).IsRequired();

            // VersionOperatingSystem 1 -- 0..* Ticket
            entity.HasOne(t => t.VersionOperatingSystem)
                  .WithMany(vos => vos.Tickets)
                  .HasForeignKey(t => t.VersionOperatingSystemId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Status 1 -- 0..* Ticket
            entity.HasOne(t => t.Status)
                  .WithMany(s => s.Tickets)
                  .HasForeignKey(t => t.StatusId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        SeedData.SeedStatuses(modelBuilder);
    }
}
