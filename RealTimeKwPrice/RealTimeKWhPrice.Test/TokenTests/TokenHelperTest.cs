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
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "test",
                Role = RoleEnums.Roles.Admin
            };

            var basePath = Directory.GetCurrentDirectory();
            var jsonFilePath = Path.Combine(basePath, "..", "..", "..", "..", "Presentation");
            //C:\dev\RealTimeKwPrice\RealTimeKwPrice\Presentation\appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(jsonFilePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var tokenHelper = new TokenHelper(configuration);
            // Act
            var result = tokenHelper.GenerateToken(user);
            // Assert
            Assert.NotNull(result);
        }
    }
}
