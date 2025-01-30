using Application.DataValidation.ExplicitWordList;
using Application.DTO.User;
using Application.Users.Commands.CreateUser;
using Domain.Interfaces;
using Domain.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace RealTimeKWhPrice.Test.UserTests
{
    public class CreateUserHandlerTest 
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<CreateUserCommandHandler> _logger;
        private CreateUserCommandHandler _handler;
        private readonly ILoggerRepository _loggerToDatabase;
        public CreateUserHandlerTest()
        {
            _userManager = A.Fake<UserManager<User>>();
            _logger = A.Fake<ILogger<CreateUserCommandHandler>>();
            _loggerToDatabase = A.Fake<ILoggerRepository>();
            _handler = new CreateUserCommandHandler(_userManager, _logger, _loggerToDatabase);
        }

        [Fact]
        [Trait("UserTests", "CreateUser")]
        public async Task CreateUser_WithValidData_ShouldReturnUser()
        {
            // Arrange
            var basePath = AppContext.BaseDirectory;
            var jsonFilePath = Path.Combine(basePath, "..", "..", "..", "..", "Application", "DataValidation", "ExplicitWordList", "ExplicitWordsJson", "explicitWords.json");
            CheckForExplicitWord checkForExplicitWord = new CheckForExplicitWord(jsonFilePath);
            
            _handler = new CreateUserCommandHandler(_userManager, _logger, _loggerToDatabase);
            
            var userDto = new CreateUserDto(
                userName: "User",
                email: "User@hotmail.com",
                password: "Password123141!#%&",
                phoneNumber: "1234567890",
                location: "Location"
            );
            var createdUser = new User
            {
                Id = Guid.NewGuid(),
                UserName = userDto.UserName,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                CreatedAt = userDto.CreatedAt,
                Role = RoleEnums.Roles.User,
                Location = userDto.Location,
                PriceList = new List<ElectricityPrice> { }
            };
            A.CallTo(() => _userManager.CreateAsync(A<User>._, A<string>._))
                .Returns(Task.FromResult(IdentityResult.Success));
            A.CallTo(() => _userManager.AddToRoleAsync(A<User>._, A<string>._))
                .Returns(Task.FromResult(IdentityResult.Success));

            // Act
            var result = await _handler.Handle(new CreateUserCommand(userDto), CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(createdUser.UserName, result.Data.UserName);
        }

        [Fact]
        [Trait("UserTests", "CreateUser")]
        public async Task CreateUser_WithInvalidData_ShouldReturnError()
        {
            // Arrange
            var userDto = new CreateUserDto(
                userName: "Use",
                email: "User@Gmail.com",
                password: "Password",
                phoneNumber: "1234",
                location: "Location"
            );
            var createdUser = new User
            {
                Id = Guid.NewGuid(),
                UserName = userDto.UserName,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                CreatedAt = userDto.CreatedAt,
                Role = RoleEnums.Roles.User,
                Location = userDto.Location,
                PriceList = new List<ElectricityPrice> { }
            };
            var basePath = AppContext.BaseDirectory;
            var jsonFilePath = Path.Combine(basePath, "..", "..", "..", "..", "Application", "DataValidation", "ExplicitWordList", "ExplicitWordsJson", "explicitWords.json");
            CheckForExplicitWord checkForExplicitWord = new CheckForExplicitWord(jsonFilePath);


            A.CallTo(() => _userManager.CreateAsync(A<User>._, A<string>._))
                .Returns(Task.FromResult(IdentityResult.Failed()));
            A.CallTo(() => _userManager.AddToRoleAsync(A<User>._, A<string>._))
                .Returns(Task.FromResult(IdentityResult.Failed()));

            // Act
            var result = await _handler.Handle(new CreateUserCommand(userDto), CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
        }
    }
}