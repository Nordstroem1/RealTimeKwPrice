using Application.Users.Commands.DeleteUser;
using Domain.Interfaces;
using Domain.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Domain.Models.RoleEnums;

namespace RealTimeKWhPrice.Test.UserTests
{
    public class DeleteUserHandlerTests
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ILogger<DeleteUserCommandHandler> _logger;
        private readonly ILoggerRepository _loggerToDatabase;
        private readonly DeleteUserCommandHandler _handler;

        public DeleteUserHandlerTests()
        {
            _userRepository = A.Fake<IGenericRepository<User>>();
            _userManager = A.Fake<UserManager<User>>();
            _roleManager = A.Fake<RoleManager<IdentityRole<Guid>>>();
            _logger = A.Fake<ILogger<DeleteUserCommandHandler>>();
            _loggerToDatabase = A.Fake<ILoggerRepository>();
            _handler = new DeleteUserCommandHandler(_userRepository, _roleManager, _userManager, _logger, _loggerToDatabase);
        }

        [Fact]
        public async Task DeleteUser_WithValidData_ShouldReturnSuccess()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                UserName = "User",
                Email = "user@example.com",
                PhoneNumber = "1234567890",
                Role = Roles.User,
                Location = "Location",
                CreatedAt = DateTime.UtcNow
            };

            var deleteUserCommand = new DeleteUserCommand(userId, user);

            A.CallTo(() => _userManager.FindByIdAsync(userId.ToString())).Returns(Task.FromResult(user));
            A.CallTo(() => _userManager.DeleteAsync(user)).Returns(Task.FromResult(IdentityResult.Success));

            var result = await _handler.Handle(deleteUserCommand, CancellationToken.None);

            Assert.True(result.Succeeded);
            Assert.Equal(user, result.Data);
        }

        [Fact]
        public async Task DeleteUser_WithInvalidData_ShouldReturnFail()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                UserName = "User",
                Email = "user@example.com",
                PhoneNumber = "1234567890",
                Role = Roles.User,
                Location = "Location",
                CreatedAt = DateTime.UtcNow
            };

            var deleteUserCommand = new DeleteUserCommand(userId, user);

            A.CallTo(() => _userManager.FindByIdAsync(userId.ToString())).Returns(Task.FromResult<User>(null));

            var result = await _handler.Handle(deleteUserCommand, CancellationToken.None);

            Assert.False(result.Succeeded);
            Assert.Equal("User not found", result.ErrorMessage);
        }

        [Fact]
        public async Task DeleteAdmin_WithValidData_ShouldReturnSuccess()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                UserName = "Admin",
                Email = "admin@example.com",
                PhoneNumber = "1234567890",
                Role = Roles.Admin,
                Location = "Location",
                CreatedAt = DateTime.UtcNow
            };

            var deleteUserCommand = new DeleteUserCommand(userId, user);

            A.CallTo(() => _userManager.FindByIdAsync(userId.ToString())).Returns(Task.FromResult(user));
            A.CallTo(() => _userManager.DeleteAsync(user)).Returns(Task.FromResult(IdentityResult.Success));

            var result = await _handler.Handle(deleteUserCommand, CancellationToken.None);

            Assert.True(result.Succeeded);
            Assert.Equal(user, result.Data);
        }

        [Fact]
        public async Task DeleteAdmin_WithInvalidData_ShouldReturnFail()
        {
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                UserName = "Admin",
                Email = "admin@example.com",
                PhoneNumber = "1234567890",
                Role = Roles.Admin,
                Location = "Location",
                CreatedAt = DateTime.UtcNow
            };

            var deleteUserCommand = new DeleteUserCommand(userId, user);

            A.CallTo(() => _userManager.FindByIdAsync(userId.ToString())).Returns(Task.FromResult<User>(null));

            var result = await _handler.Handle(deleteUserCommand, CancellationToken.None);

            Assert.False(result.Succeeded);
            Assert.Equal("User not found", result.ErrorMessage);
        }
    }
}