using Application.TokenHelper;
using Domain.Models;
using Microsoft.Extensions.Configuration;

namespace RealTimeKWhPrice.Test.TokenTests
{
    public class TokenHelperTest
    {
        [Fact]
        [Trait("TokenTests", "Token")]
        public void GenerateToken_WhenCalled_ReturnsToken()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "test",
                Role = RoleEnums.Roles.Admin
            };

            var basePath = Directory.GetCurrentDirectory();
            var jsonFilePath = Path.Combine(basePath, "..", "..", "..", "..", "Presentation");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(jsonFilePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var tokenHelper = new TokenHelper(configuration);
            var result = tokenHelper.GenerateToken(user);
            Assert.NotNull(result);
        }
    }
}
