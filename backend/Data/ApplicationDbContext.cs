using Enhanzer.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Enhanzer.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<ItemMaster> Items => Set<ItemMaster>();
    public DbSet<PurchaseBillHeader> PurchaseBillHeaders => Set<PurchaseBillHeader>();
    public DbSet<PurchaseBillLine> PurchaseBillLines => Set<PurchaseBillLine>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Location>(entity =>
        {
            entity.ToTable("Locations");
            entity.HasKey(location => location.Id);
            entity.HasIndex(location => location.Code).IsUnique();
            entity.Property(location => location.Code)
                .HasMaxLength(100)
                .IsRequired();
            entity.Property(location => location.Name)
                .HasMaxLength(200)
                .IsRequired();
        });

        modelBuilder.Entity<ItemMaster>(entity =>
        {
            entity.ToTable("Items");
            entity.HasKey(item => item.Id);
            entity.HasIndex(item => item.Name).IsUnique();
            entity.Property(item => item.Name)
                .HasMaxLength(100)
                .IsRequired();
        });

        modelBuilder.Entity<PurchaseBillHeader>(entity =>
        {
            entity.ToTable("PurchaseBillHeaders");
            entity.HasKey(header => header.Id);
            entity.HasIndex(header => header.BillNumber).IsUnique();
            entity.HasIndex(header => header.OfflineClientId).IsUnique();
            entity.Property(header => header.BillNumber)
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(header => header.SupplierName).HasMaxLength(150);
            entity.Property(header => header.ReferenceNo).HasMaxLength(100);
            entity.Property(header => header.SyncStatus).HasMaxLength(20).IsRequired();
            entity.Property(header => header.Notes).HasMaxLength(1000);
            entity.Property(header => header.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(header => header.TotalCost).HasColumnType("decimal(18,2)");
            entity.HasMany(header => header.Items)
                .WithOne(item => item.PurchaseBillHeader)
                .HasForeignKey(item => item.PurchaseBillHeaderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PurchaseBillLine>(entity =>
        {
            entity.ToTable("PurchaseBillLines");
            entity.HasKey(line => line.Id);
            entity.Property(line => line.ItemName).HasMaxLength(100).IsRequired();
            entity.Property(line => line.BatchCode).HasMaxLength(100).IsRequired();
            entity.Property(line => line.Cost).HasColumnType("decimal(18,2)");
            entity.Property(line => line.Price).HasColumnType("decimal(18,2)");
            entity.Property(line => line.DiscountPercentage).HasColumnType("decimal(18,2)");
            entity.Property(line => line.TotalCost).HasColumnType("decimal(18,2)");
            entity.Property(line => line.TotalSelling).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("Audit_Logs");
            entity.HasKey(log => log.Id);
            entity.Property(log => log.Entity).HasMaxLength(100).IsRequired();
            entity.Property(log => log.Action).HasMaxLength(50).IsRequired();
        });
    }
}
