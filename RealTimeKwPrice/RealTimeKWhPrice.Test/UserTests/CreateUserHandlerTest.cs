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
        private readonly ILoggerRepository _loggerToDatabase;
        private readonly CheckForExplicitWord _checkForExplicitWord;
        private CreateUserCommandHandler _handler;

        public CreateUserHandlerTest()
        {
            _userManager = A.Fake<UserManager<User>>();
            _logger = A.Fake<ILogger<CreateUserCommandHandler>>();
            _loggerToDatabase = A.Fake<ILoggerRepository>();

            var explicitWordLogger = A.Fake<ILogger<CheckForExplicitWord>>();

            var basePath = AppContext.BaseDirectory;
            var jsonFilePath = Path.Combine(basePath, "..", "..", "..", "..", "Application", "DataValidation", "ExplicitWordList", "ExplicitWordsJson", "explicitWords.json");

            _checkForExplicitWord = new CheckForExplicitWord(jsonFilePath, explicitWordLogger);

            _handler = new CreateUserCommandHandler(_userManager, _logger, _loggerToDatabase, explicitWordLogger);
        }

        [Fact]
        [Trait("UserTests", "CreateUser")]
        public async Task CreateUser_WithValidData_ShouldReturnUser()
        {
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

            var result = await _handler.Handle(new CreateUserCommand(userDto), CancellationToken.None);

            Assert.True(result.Succeeded);
            Assert.Equal(createdUser.UserName, result.Data.UserName);
        }

        [Fact]
        [Trait("UserTests", "CreateUser")]
        public async Task CreateUser_WithInvalidData_ShouldReturnError()
        {
            var userDto = new CreateUserDto(
                userName: "Use", 
                email: "User@Gmail.com",
                password: "Password",
                phoneNumber: "1234",
                location: "Location"
            );

            A.CallTo(() => _userManager.CreateAsync(A<User>._, A<string>._))
                .Returns(Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "Username is too short" })));
            A.CallTo(() => _userManager.AddToRoleAsync(A<User>._, A<string>._))
                .Returns(Task.FromResult(IdentityResult.Failed()));

            var result = await _handler.Handle(new CreateUserCommand(userDto), CancellationToken.None);

            Assert.False(result.Succeeded);
            Assert.Equal("Failed to create user: Username is too short", result.ErrorMessage); 
        }
    }
}