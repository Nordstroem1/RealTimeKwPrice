using Application.Commands;
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
    public class UpdateUserHandlerTest
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ILogger<UpdateUserCommandHandler> _logger;
        private readonly ILoggerRepository _loggerToDatabase;
        private readonly UpdateUserCommandHandler _handler;

        public UpdateUserHandlerTest()
        {
            _userRepository = A.Fake<IGenericRepository<User>>();
            _userManager = A.Fake<UserManager<User>>();
            _roleManager = A.Fake<RoleManager<IdentityRole<Guid>>>();
            _logger = A.Fake<ILogger<UpdateUserCommandHandler>>();
            _loggerToDatabase = A.Fake<ILoggerRepository>();
            _handler = new UpdateUserCommandHandler(_userRepository, _roleManager, _userManager, _logger, _loggerToDatabase);
        }

        [Fact]
        public async Task UpdateUser_WithValidData_ShouldReturnSuccess()
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

            var updateUserCommand = new UpdateUserCommand(userId, user);

            A.CallTo(() => _userRepository.GetByIdAsync(userId)).Returns(Task.FromResult(user));
            A.CallTo(() => _userManager.PasswordHasher.HashPassword(user, user.PasswordHash)).Returns("hashedPassword");
            A.CallTo(() => _userManager.GetRolesAsync(user)).Returns(Task.FromResult<IList<string>>(new List<string> { "User" }));
            A.CallTo(() => _userManager.UpdateAsync(user)).Returns(Task.FromResult(IdentityResult.Success));

            var result = await _handler.Handle(updateUserCommand, CancellationToken.None);

            Assert.True(result.Succeeded);
            Assert.Equal(user, result.Data);
        }

        [Fact]
        public async Task UpdateUser_WithInvalidData_ShouldReturnFail()
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

            var updateUserCommand = new UpdateUserCommand(userId, user);

            A.CallTo(() => _userRepository.GetByIdAsync(userId)).Returns(Task.FromResult<User>(null));

            var result = await _handler.Handle(updateUserCommand, CancellationToken.None);

            Assert.False(result.Succeeded);
            Assert.Equal("User not found", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateAdmin_WithValidData_ShouldReturnSuccess()
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

            var updateUserCommand = new UpdateUserCommand(userId, user);

            A.CallTo(() => _userRepository.GetByIdAsync(userId)).Returns(Task.FromResult(user));
            A.CallTo(() => _userManager.PasswordHasher.HashPassword(user, user.PasswordHash)).Returns("hashedPassword");
            A.CallTo(() => _userManager.GetRolesAsync(user)).Returns(Task.FromResult<IList<string>>(new List<string> { "Admin" }));
            A.CallTo(() => _userManager.UpdateAsync(user)).Returns(Task.FromResult(IdentityResult.Success));

            var result = await _handler.Handle(updateUserCommand, CancellationToken.None);

            Assert.True(result.Succeeded);
            Assert.Equal(user, result.Data);
        }

        [Fact]
        public async Task UpdateAdmin_WithInvalidData_ShouldReturnFail()
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

            var updateUserCommand = new UpdateUserCommand(userId, user);

            A.CallTo(() => _userRepository.GetByIdAsync(userId)).Returns(Task.FromResult<User>(null));

            var result = await _handler.Handle(updateUserCommand, CancellationToken.None);

            Assert.False(result.Succeeded);
            Assert.Equal("User not found", result.ErrorMessage);
        }
    }
}