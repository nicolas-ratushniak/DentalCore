﻿using DentalCore.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DentalCore.Data;

public class AppDbContext : DbContext
{
    public DbSet<Allergy> Allergies { get; set; }
    public DbSet<PatientAllergy> PatientAllergies { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Disease> Diseases { get; set; }
    public DbSet<PatientDisease> PatientDiseases { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Procedure> Procedures { get; set; }
    public DbSet<TreatmentItem> TreatmentItems { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Visit> Visits { get; set; }
    public DbSet<Phone> Phones { get; set; }

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Allergy>()
            .HasMany(e => e.Patients)
            .WithMany(e => e.Allergies)
            .UsingEntity<PatientAllergy>(
                pa => pa.Property(e => e.CreatedOn)
                    .HasDefaultValueSql("GETDATE()"));

        modelBuilder.Entity<Disease>()
            .HasMany(e => e.Patients)
            .WithMany(e => e.Diseases)
            .UsingEntity<PatientDisease>(
                pa => pa.Property(e => e.CreatedOn)
                    .HasDefaultValueSql("GETDATE()"));

        modelBuilder.Entity<TreatmentItem>()
            .HasKey(nameof(TreatmentItem.VisitId), nameof(TreatmentItem.ProcedureId));

        base.OnModelCreating(modelBuilder);
    }
}