//using Application.TokenHelper;
//using Domain.Models;
//using FakeItEasy;
//using Microsoft.Extensions.Configuration;

//namespace RealTimeKWhPrice.Test.TokenHelperTests
//{
//    public class TokenHelperTest
//    {
//        private readonly TokenHelper _tokenHelper;
//        private readonly IConfiguration _configuration;
//        public TokenHelperTest()
//        {
//            _configuration = A.Fake<IConfiguration>();
//            A.CallTo(() => _configuration["Jwt:Key"]).Returns("your_secret_key");
//            A.CallTo(() => _configuration["Jwt:Issuer"]).Returns("your_issuer");
//            A.CallTo(() => _configuration["Jwt:Audience"]).Returns("your_audience");

//            _tokenHelper = new TokenHelper(_configuration);
//        }

//        [Fact]
//        [Trait("TokenHelper", "GenerateToken")]
//        public void GenerateToken_WithValidData_ShouldReturnToken()
//        {
//            // Arrange
//            var user = new User
//            {
//                Id = Guid.NewGuid(),
//                UserName = "User",
//                Email = "email@hotmail.com",
//                PasswordHash = "Password",
//                PhoneNumber = "1234567890",
//                CreatedAt = DateTime.Now,
//                Role = RoleEnums.Roles.User,
//                Location = "Location",
//            };
//            //Act
//            var token = _tokenHelper.GenerateToken(user);

//            //Assert
//            Assert.NotNull(token);
//            Assert.IsType<string>(token);
//        }

//        [Fact]
//        [Trait("TokenHelper", "ValidateToken")]
//        public void ValidateToken_ShouldReturnTrueForValidToken()
//        {
//            // Arrange
//            var user = new User
//            {
//                Id = Guid.NewGuid(),
//                UserName = "User",
//                Email = "email@hotmail.com",
//                PhoneNumber = "1234567890",
//                CreatedAt = DateTime.Now,
//                Role = RoleEnums.Roles.User,
//                Location = "Location",
//            };
//            var token = _tokenHelper.GenerateToken(user);

//            // Act
//            var isValid = _tokenHelper.ValidateToken(token);

//            // Assert
//            Assert.True(isValid);
//        }
//    }
//}
