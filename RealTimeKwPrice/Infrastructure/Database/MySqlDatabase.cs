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
        public DbSet<KiloWattPrice> KiloWattPrices { get; set; }
        public DbSet<RoleEnums> RoleEnums { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
