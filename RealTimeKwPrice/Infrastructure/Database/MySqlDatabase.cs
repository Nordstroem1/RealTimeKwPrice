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
        }
    }
}
