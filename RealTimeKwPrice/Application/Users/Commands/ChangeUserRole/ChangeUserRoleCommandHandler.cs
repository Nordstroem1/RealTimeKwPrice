using Application.DTO.Role;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
namespace Application.Users.Commands.ChangeUserRole
{
    public class ChangeUserRoleCommandHandler : IRequestHandler<ChangeUserRoleCommand, OperationResult<UserRoleResultDTO>>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ILoggerRepository _loggerToDatabase;
        private readonly ILogger<ChangeUserRoleCommandHandler> _logger;

        public ChangeUserRoleCommandHandler(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager, ILoggerRepository loggerToDatabase, ILogger<ChangeUserRoleCommandHandler> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _loggerToDatabase = loggerToDatabase;
            _logger = logger;
        }


        public async Task<OperationResult<UserRoleResultDTO>> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.ChangeUserRoleDTO.UserId.ToString());
                if (user == null)
                {
                    _logger.LogError("User with ID {UserId} not found", request.ChangeUserRoleDTO.UserId);

                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(ChangeUserRoleCommandHandler),
                        WhatWentWrong = $"User not found with ID {request.ChangeUserRoleDTO.UserId}",
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(Handle)
                    });

                    return OperationResult<UserRoleResultDTO>.Fail("User not found.", nameof(Handle));
                }

                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Contains(request.ChangeUserRoleDTO.NewRole))
                {
                    _logger.LogError("User {UserId} already has the role {Role}", user.Id, request.ChangeUserRoleDTO.NewRole);

                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(ChangeUserRoleCommandHandler),
                        WhatWentWrong = $"User already has the role {request.ChangeUserRoleDTO.NewRole}",
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(Handle)
                    });

                    return OperationResult<UserRoleResultDTO>.Fail($"User already has the role {request.ChangeUserRoleDTO.NewRole}.", nameof(Handle));
                }

                var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeRolesResult.Succeeded)
                {
                    var errors = string.Join(", ", removeRolesResult.Errors.Select(e => e.Description));
                    _logger.LogError($"Failed to remove current roles for user {user.Id}: {errors}");

                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(ChangeUserRoleCommandHandler),
                        WhatWentWrong = errors,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(Handle)
                    });

                    return OperationResult<UserRoleResultDTO>.Fail("Failed to remove current roles.", nameof(Handle));
                }

                var addRoleResult = await _userManager.AddToRoleAsync(user, request.ChangeUserRoleDTO.NewRole);
                if (!addRoleResult.Succeeded)
                {
                    var errors = string.Join(", ", addRoleResult.Errors.Select(e => e.Description));
                    _logger.LogError($"Failed to add new role to user {user.Id}: {errors}");

                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(ChangeUserRoleCommandHandler),
                        WhatWentWrong = errors,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(Handle)
                    });

                    return OperationResult<UserRoleResultDTO>.Fail("Failed to add new role.", nameof(Handle));
                }

                var result = new UserRoleResultDTO
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    CurrentRole = request.ChangeUserRoleDTO.NewRole
                };

                _logger.LogInformation("Role for user {UserId} changed to {Role} successfully", user.Id, request.ChangeUserRoleDTO.NewRole);

                return OperationResult<UserRoleResultDTO>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while changing role for user: {ex.Message}");

                await _loggerToDatabase.LogErrorAsync(new Logger
                {
                    Location = nameof(ChangeUserRoleCommandHandler),
                    WhatWentWrong = ex.Message,
                    TimeStamp = DateTime.UtcNow,
                    Function = nameof(Handle)
                });

                return OperationResult<UserRoleResultDTO>.Fail($"An unexpected error occurred: {ex.Message}", nameof(Handle));
            }
        }
    }
}
