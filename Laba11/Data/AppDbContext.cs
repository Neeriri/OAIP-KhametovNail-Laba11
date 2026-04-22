using Microsoft.EntityFrameworkCore;
using Laba11.Models;

namespace Laba11.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=PracticeDb;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Студент
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DateOfBirth).IsRequired();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(100);
                entity.Property(e => e.University).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Specialty).IsRequired().HasMaxLength(100);

                entity.HasIndex(e => e.Email).IsUnique();

                // Связь Студент -> Компания (многие к одному)
                entity.HasOne(e => e.Company)
                      .WithMany(c => c.Students)
                      .HasForeignKey(e => e.CompanyId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Связь Студент -> Отчёты (один ко многим)
                entity.HasMany(e => e.Reports)
                      .WithOne(r => r.Student)
                      .HasForeignKey(r => r.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Компания
            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Industry).IsRequired().HasMaxLength(100);
                entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            });

            // Отчёт
            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.SubmissionDate).IsRequired();
                entity.Property(e => e.Topic).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Grade).IsRequired();

                entity.HasOne(e => e.Company)
                      .WithMany()
                      .HasForeignKey(e => e.CompanyId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
