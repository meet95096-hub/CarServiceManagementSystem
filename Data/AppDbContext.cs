using Microsoft.EntityFrameworkCore;
using web.net.Models;

namespace web.net.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Mechanic> Mechanics => Set<Mechanic>();
    public DbSet<ServicePackage> ServicePackages => Set<ServicePackage>();
    public DbSet<ServiceAppointment> ServiceAppointments => Set<ServiceAppointment>();
    public DbSet<RepairJobCard> RepairJobCards => Set<RepairJobCard>();
    public DbSet<RepairLaborItem> RepairLaborItems => Set<RepairLaborItem>();
    public DbSet<RepairPartItem> RepairPartItems => Set<RepairPartItem>();
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Vehicle indexes
        modelBuilder.Entity<Vehicle>()
            .HasIndex(v => v.LicensePlate)
            .IsUnique();

        // Unique indexes for codes
        modelBuilder.Entity<ServiceAppointment>()
            .HasIndex(a => a.AppointmentCode)
            .IsUnique();

        modelBuilder.Entity<RepairJobCard>()
            .HasIndex(j => j.JobCardNumber)
            .IsUnique();

        modelBuilder.Entity<Invoice>()
            .HasIndex(i => i.InvoiceNumber)
            .IsUnique();

        // Decimal precision configurations
        foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,2)");
        }

        // Job Card -> Items relationships
        modelBuilder.Entity<RepairJobCard>()
            .HasMany(j => j.LaborItems)
            .WithOne(l => l.RepairJobCard!)
            .HasForeignKey(l => l.RepairJobCardId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RepairJobCard>()
            .HasMany(j => j.PartItems)
            .WithOne(p => p.RepairJobCard!)
            .HasForeignKey(p => p.RepairJobCardId)
            .OnDelete(DeleteBehavior.Cascade);

        // Job Card -> Invoice (1:1 optional)
        modelBuilder.Entity<RepairJobCard>()
            .HasOne(j => j.Invoice)
            .WithOne(i => i.RepairJobCard!)
            .HasForeignKey<Invoice>(i => i.RepairJobCardId)
            .OnDelete(DeleteBehavior.Restrict);

        // Job Card -> Service Appointment (1:1 optional)
        modelBuilder.Entity<RepairJobCard>()
            .HasOne(j => j.ServiceAppointment)
            .WithOne(a => a.LinkedRepairJob!)
            .HasForeignKey<RepairJobCard>(j => j.ServiceAppointmentId)
            .OnDelete(DeleteBehavior.SetNull);

        // Vehicle relationships
        modelBuilder.Entity<Vehicle>()
            .HasMany(v => v.Appointments)
            .WithOne(a => a.Vehicle!)
            .HasForeignKey(a => a.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Vehicle>()
            .HasMany(v => v.RepairJobs)
            .WithOne(r => r.Vehicle!)
            .HasForeignKey(r => r.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Vehicle>()
            .HasMany(v => v.Invoices)
            .WithOne(i => i.Vehicle!)
            .HasForeignKey(i => i.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
