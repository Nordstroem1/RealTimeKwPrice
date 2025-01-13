using Application.DTO.Role;
using Application.DTO.Role.SuperAdmin;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.ChangeAdminRoleAsSuperAdmin
{
    public class ChangeRoleAsSuperAdminHandler : IRequestHandler<ChangeRoleAsSuperAdminCommand, OperationResult<ChangeRoleAsSuperAdminDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ChangeRoleAsSuperAdminHandler> _logger;
        public ChangeRoleAsSuperAdminHandler(UserManager<User> userManager, ILogger<ChangeRoleAsSuperAdminHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }
        public async Task<OperationResult<ChangeRoleAsSuperAdminDto>> Handle(ChangeRoleAsSuperAdminCommand request, CancellationToken cancellationToken)
        {
            var foundUser = await _userManager.FindByIdAsync(request.RoleDTO.UserId.ToString());
            if (foundUser == null)
            {
                _logger.LogError($"User with ID {request.RoleDTO.UserId} not found.");
                return OperationResult<ChangeRoleAsSuperAdminDto>.Fail("User not found", "ChangeRoleAsSuperAdminHandler");
            }

            var currentRoles = await _userManager.GetRolesAsync(foundUser);

            if (foundUser.Role == RoleEnums.Roles.SuperAdmin)
            {
                _logger.LogError($"User with ID {request.RoleDTO.UserId} is a super admin.");
                return OperationResult<ChangeRoleAsSuperAdminDto>.Fail("Can not remove a SuperAdmin", "Application");
            }

            var removeRolesResult = await _userManager.RemoveFromRolesAsync(foundUser, currentRoles);
            if (!removeRolesResult.Succeeded)
            {
                _logger.LogError($"Failed to remove roles from user with ID {request.RoleDTO.UserId}");
                return OperationResult<ChangeRoleAsSuperAdminDto>.Fail("Failed to remove roles", "ChangeRoleAsSuperAdminHandler");
            }

            var addRoleResult = await _userManager.AddToRoleAsync(foundUser, request.RoleDTO.NewRole);
            if (!addRoleResult.Succeeded)
            {
                _logger.LogError($"Failed to add role {request.RoleDTO.NewRole} to user with ID {request.RoleDTO.UserId}.");
                return OperationResult<ChangeRoleAsSuperAdminDto>.Fail("Failed to add new role to user", "ChangeRoleAsSuperAdminHandler");
            }

            _logger.LogInformation($"Successfully changed role for user with ID {request.RoleDTO.UserId} to {request.RoleDTO.NewRole}.");
            return OperationResult<ChangeRoleAsSuperAdminDto>.Success(new ChangeRoleAsSuperAdminDto
            {
                UserId = foundUser.Id,
                NewRole = request.RoleDTO.NewRole
            });
        }
    }
}
