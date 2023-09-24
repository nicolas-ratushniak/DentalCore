using DentalCore.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DentalCore.Data;

public class AppDbContext : DbContext
{
    public DbSet<Allergy> Allergies { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Disease> Diseases { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Procedure> Procedures { get; set; }
    public DbSet<TreatmentItem> TreatmentItems { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Visit> Visits { get; set; }

    public AppDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Allergy>(a =>
            a.HasKey(nameof(Allergy.PatientId), nameof(Allergy.Name)));

        modelBuilder.Entity<TreatmentItem>(t =>
            t.HasKey(nameof(TreatmentItem.VisitId), nameof(TreatmentItem.ProcedureId)));

        base.OnModelCreating(modelBuilder);
    }
}