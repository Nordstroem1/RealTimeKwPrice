using MediatR;
using Domain.Interfaces;
using Domain.Models;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Commands
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, OperationResult<User>>
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ILogger<UpdateUserCommandHandler> _logger;

        public UpdateUserCommandHandler(IGenericRepository<User> userRepository,
            RoleManager<IdentityRole<Guid>> roleManager, UserManager<User> userManager, ILogger<UpdateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<OperationResult<User>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingUser = await _userRepository.GetByIdAsync(request.Id);
                if (existingUser == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", request.Id);
                    return OperationResult<User>.Fail("User not found", nameof(UpdateUserCommandHandler));
                }

                existingUser.UserName = request.User.UserName;
                existingUser.Email = request.User.Email;
                existingUser.PhoneNumber = request.User.PhoneNumber;
                existingUser.PasswordHash = _userManager.PasswordHasher.HashPassword(existingUser, request.User.PasswordHash);
                existingUser.Location = request.User.Location;

                var currentRoles = await _userManager.GetRolesAsync(existingUser);
                var newRole = request.User.Role.ToString();
                if (!currentRoles.Contains(newRole))
                {
                    await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);
                    await _userManager.AddToRoleAsync(existingUser, newRole);
                }

                var result = await _userManager.UpdateAsync(existingUser);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to update user with ID {UserId}", request.Id);
                    return OperationResult<User>.Fail("Failed to update user", nameof(UpdateUserCommandHandler));
                }

                _logger.LogInformation("User with ID {UserId} updated successfully", request.Id);
                return OperationResult<User>.Success(existingUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user with ID {UserId}", request.Id);
                return OperationResult<User>.Fail("An error occurred while updating the user", nameof(UpdateUserCommandHandler));
            }
        }
    }
}