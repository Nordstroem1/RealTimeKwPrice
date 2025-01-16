using MediatR;
using Domain.Models;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Users.Commands.DeleteUserAsAdmin;

namespace Application.Users.Commands.DeleteUser
{
    public class DeleteUserCommandHandler :
        IRequestHandler<DeleteUserCommand, OperationResult<User>>,
        IRequestHandler<DeleteUserAsAdminCommand, OperationResult<User>>
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<DeleteUserCommandHandler> _logger;

        public DeleteUserCommandHandler(IGenericRepository<User> userRepository, RoleManager<IdentityRole<Guid>> roleManager, UserManager<User> userManager, ILogger<DeleteUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<OperationResult<User>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            return await HandleDeleteUser(request.Id);
        }

        public async Task<OperationResult<User>> Handle(DeleteUserAsAdminCommand request, CancellationToken cancellationToken)
        {
            return await HandleDeleteUser(request.Id);
        }

        private async Task<OperationResult<User>> HandleDeleteUser(Guid userId)
        {
            try
            {
                var existingUser = await _userManager.FindByIdAsync(userId.ToString());
                if (existingUser == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", userId);
                    return OperationResult<User>.Fail("User not found", nameof(DeleteUserCommandHandler));
                }

                var result = await _userManager.DeleteAsync(existingUser);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to delete user with ID {UserId}", userId);
                    return OperationResult<User>.Fail("Failed to delete user", nameof(DeleteUserCommandHandler));
                }

                _logger.LogInformation("User with ID {UserId} deleted successfully", userId);
                return OperationResult<User>.Success(existingUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting user with ID {UserId}", userId);
                return OperationResult<User>.Fail("An error occurred while deleting the user", nameof(DeleteUserCommandHandler));
            }
        }
    }
}