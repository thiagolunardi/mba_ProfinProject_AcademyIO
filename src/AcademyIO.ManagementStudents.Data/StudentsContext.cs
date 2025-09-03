﻿using AcademyIO.ManagementStudents.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AcademyIO.Core.Data;

namespace AcademyIO.ManagementStudents.Data
{
    public class StudentsContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>, IUnitOfWork
    {
        public StudentsContext(DbContextOptions<StudentsContext> options) : base(options) { }

        public DbSet<User> SystemUsers { get; set; }

        public DbSet<Certification> Certifications { get; set; }

        public DbSet<Registration> Registrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RegistrationConfiguration());
            modelBuilder.ApplyConfiguration(new CertificationConfiguration());
        }
        public async Task<bool> Commit()
        {
            foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("CreatedDate") != null))
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedDate").CurrentValue = DateTime.Now;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property("CreatedDate").IsModified = false;
                }
            }

            return await base.SaveChangesAsync() > 0;
        }
    }
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(a => a.Id);
            builder.ToTable("Users");
        }
    }

    public class CertificationConfiguration : IEntityTypeConfiguration<Certification>
    {
        public void Configure(EntityTypeBuilder<Certification> builder)
        {
            builder.HasKey(a => a.Id);
            builder.ToTable("Certifications");

            builder.HasOne(ct => ct.Student)
                 .WithMany()
                 .HasForeignKey(ct => ct.StudentId)
                 .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class RegistrationConfiguration : IEntityTypeConfiguration<Registration>
    {
        public void Configure(EntityTypeBuilder<Registration> builder)
        {
            builder.HasKey(a => a.Id);
            builder.ToTable("Registrations");

            builder.HasOne(r => r.Student)
                 .WithMany()
                 .HasForeignKey(r => r.StudentId)
                 .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
