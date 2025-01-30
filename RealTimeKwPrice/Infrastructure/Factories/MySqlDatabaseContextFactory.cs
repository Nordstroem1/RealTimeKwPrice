using Infrastructure.Database;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Factories
{
    public class MySqlDatabaseContextFactory : IDesignTimeDbContextFactory<MySqlDatabase>
    {
        public MySqlDatabase CreateDbContext(string[] args)
        {
            var basePath = AppContext.BaseDirectory;
            var jsonFilePath = Path.Combine(basePath, "..", "..", "..", "..", "Presentation");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(jsonFilePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<MySqlDatabase>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString, b => b.MigrationsAssembly("Infrastructure"));

            return new MySqlDatabase(optionsBuilder.Options);
        }
    }
}
