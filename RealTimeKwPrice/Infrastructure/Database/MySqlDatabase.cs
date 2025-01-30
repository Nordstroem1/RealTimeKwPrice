using Microsoft.EntityFrameworkCore;
using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Database
{
    public class MySqlDatabase : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public MySqlDatabase(DbContextOptions<MySqlDatabase> options) : base(options) { }
        public DbSet<ElectricityPrice> ElectricityPrices { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Logger> Loggers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("YourConnectionString",
                    b => b.MigrationsAssembly("Infrastructure"));
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfigurera precision och skala för decimal-egenskaper
            modelBuilder.Entity<ElectricityPrice>()
                .Property(e => e.EUR_per_kWh)
                .HasColumnType("decimal(18, 4)"); // Anger precision 18 och skala 4

            modelBuilder.Entity<ElectricityPrice>()
                .Property(e => e.SEK_per_kWh)
                .HasColumnType("decimal(18, 4)"); // Anger precision 18 och skala 4

            modelBuilder.Entity<ElectricityPrice>()
                .Property(e => e.EXR)
                .HasColumnType("decimal(18, 4)"); // Anger precision 18 och skala 4

            modelBuilder.Entity<Logger>(entity =>
            {
                entity.HasKey(l => l.Id); 
                entity.Property(l => l.Location).IsRequired().HasMaxLength(255);
                entity.Property(l => l.WhatWentWrong).IsRequired().HasMaxLength(1000);
                entity.Property(l => l.TimeStamp).IsRequired();
                entity.Property(l => l.Function).HasMaxLength(255);
            });
        }
    }
}
