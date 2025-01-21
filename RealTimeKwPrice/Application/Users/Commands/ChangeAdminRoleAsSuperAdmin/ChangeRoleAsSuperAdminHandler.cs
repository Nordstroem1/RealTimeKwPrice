using Application.DTO.Role;
using Application.DTO.Role.SuperAdmin;
using Domain.Interfaces;
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
        private readonly ILoggerRepository _loggerToDatabase;

        public ChangeRoleAsSuperAdminHandler(UserManager<User> userManager, ILogger<ChangeRoleAsSuperAdminHandler> logger, ILoggerRepository loggerToDatabase)
        {
            _userManager = userManager;
            _logger = logger;
            _loggerToDatabase = loggerToDatabase;
        }
        public async Task<OperationResult<ChangeRoleAsSuperAdminDto>> Handle(ChangeRoleAsSuperAdminCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var foundUser = await _userManager.FindByIdAsync(request.RoleDTO.UserId.ToString());
                if (foundUser == null)
                {
                    var errorMessage = $"User with ID {request.RoleDTO.UserId} not found.";

                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(ChangeRoleAsSuperAdminHandler),
                        WhatWentWrong = errorMessage,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(Handle)
                    });

                    _logger.LogError(errorMessage);
                    return OperationResult<ChangeRoleAsSuperAdminDto>.Fail("User not found", nameof(ChangeRoleAsSuperAdminHandler));
                }

                var currentRoles = await _userManager.GetRolesAsync(foundUser);

                if (foundUser.Role == RoleEnums.Roles.SuperAdmin)
                {
                    var errorMessage = $"User with ID {request.RoleDTO.UserId} is already a super admin.";

                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(ChangeRoleAsSuperAdminHandler),
                        WhatWentWrong = errorMessage,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(Handle)
                    });

                    _logger.LogError(errorMessage);
                    return OperationResult<ChangeRoleAsSuperAdminDto>.Fail("Cannot remove a SuperAdmin", nameof(ChangeRoleAsSuperAdminHandler));
                }

                var removeRolesResult = await _userManager.RemoveFromRolesAsync(foundUser, currentRoles);
                if (!removeRolesResult.Succeeded)
                {
                    var errors = string.Join(", ", removeRolesResult.Errors.Select(e => e.Description));

                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(ChangeRoleAsSuperAdminHandler),
                        WhatWentWrong = $"Failed to remove roles from user with ID {request.RoleDTO.UserId}: {errors}",
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(Handle)
                    });

                    _logger.LogError($"Failed to remove roles from user with ID {request.RoleDTO.UserId}: {errors}");
                    return OperationResult<ChangeRoleAsSuperAdminDto>.Fail("Failed to remove roles", nameof(ChangeRoleAsSuperAdminHandler));
                }

                var addRoleResult = await _userManager.AddToRoleAsync(foundUser, request.RoleDTO.NewRole);
                if (!addRoleResult.Succeeded)
                {
                    var errors = string.Join(", ", addRoleResult.Errors.Select(e => e.Description));

                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(ChangeRoleAsSuperAdminHandler),
                        WhatWentWrong = $"Failed to add role {request.RoleDTO.NewRole} to user with ID {request.RoleDTO.UserId}: {errors}",
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(Handle)
                    });

                    _logger.LogError($"Failed to add role {request.RoleDTO.NewRole} to user with ID {request.RoleDTO.UserId}: {errors}");
                    return OperationResult<ChangeRoleAsSuperAdminDto>.Fail("Failed to add new role to user", nameof(ChangeRoleAsSuperAdminHandler));
                }

                _logger.LogInformation($"Successfully changed role for user with ID {request.RoleDTO.UserId} to {request.RoleDTO.NewRole}.");

                return OperationResult<ChangeRoleAsSuperAdminDto>.Success(new ChangeRoleAsSuperAdminDto
                {
                    UserId = foundUser.Id,
                    NewRole = request.RoleDTO.NewRole
                });
            }
            catch (Exception ex)
            {
                var errorMessage = $"An unexpected error occurred while changing the role for user with ID {request.RoleDTO.UserId}: {ex.Message}";

                await _loggerToDatabase.LogErrorAsync(new Logger
                {
                    Location = nameof(ChangeRoleAsSuperAdminHandler),
                    WhatWentWrong = errorMessage,
                    TimeStamp = DateTime.UtcNow,
                    Function = nameof(Handle)
                });

                _logger.LogError(errorMessage);
                return OperationResult<ChangeRoleAsSuperAdminDto>.Fail($"An unexpected error occurred: {ex.Message}", nameof(ChangeRoleAsSuperAdminHandler));
            }
        }
    }
}
