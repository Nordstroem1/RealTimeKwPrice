using Application.DTO.Role;
using Application.Users.Commands.ChangeUserRole;
using Domain.Interfaces;
using Domain.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Xunit;

namespace RealTimeKWhPrice.Test.UserTests
{
    public class ChangeUserRoleHandlerTest
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ILogger<ChangeUserRoleCommandHandler> _logger;
        private readonly ILoggerRepository _loggerToDatabase;
        private readonly ChangeUserRoleCommandHandler _handler;

        public ChangeUserRoleHandlerTest()
        {
            _userManager = A.Fake<UserManager<User>>();
            _roleManager = A.Fake<RoleManager<IdentityRole<Guid>>>();
            _logger = A.Fake<ILogger<ChangeUserRoleCommandHandler>>();
            _loggerToDatabase = A.Fake<ILoggerRepository>();
            _handler = new ChangeUserRoleCommandHandler(_userManager, _roleManager, _loggerToDatabase, _logger);
        }

        [Fact]
        [Trait("UserTests", "ChangeUserRole")]
        public async Task ChangeUserRole_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, UserName = "TestUser" };
            var newRole = "Admin";
            var dto = new ChangeUserRoleDTO(userId, newRole);

            A.CallTo(() => _userManager.FindByIdAsync(userId.ToString())).Returns(Task.FromResult(user));
            A.CallTo(() => _userManager.GetRolesAsync(user)).Returns(Task.FromResult<IList<string>>(new List<string>() { "User" }));
            A.CallTo(() => _userManager.RemoveFromRolesAsync(user, A<IEnumerable<string>>._)).Returns(Task.FromResult(IdentityResult.Success));
            A.CallTo(() => _userManager.AddToRoleAsync(user, newRole)).Returns(Task.FromResult(IdentityResult.Success));

            // Act
            var result = await _handler.Handle(new ChangeUserRoleCommand(dto), CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(newRole, result.Data.CurrentRole);
        }

        [Fact]
        [Trait("UserTests", "ChangeUserRole")]
        public async Task ChangeUserRole_WithInvalidUser_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var newRole = "Admin";
            var dto = new ChangeUserRoleDTO(userId, newRole);

            A.CallTo(() => _userManager.FindByIdAsync(userId.ToString())).Returns(Task.FromResult<User>(null));

            // Act
            var result = await _handler.Handle(new ChangeUserRoleCommand(dto), CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("User not found.", result.ErrorMessage);
        }
    }
}

