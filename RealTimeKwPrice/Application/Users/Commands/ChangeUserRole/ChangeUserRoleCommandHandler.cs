using Application.DTO;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Commands.ChangeUserRole
{
    public class ChangeUserRoleCommandHandler : IRequestHandler<ChangeUserRoleCommand, OperationResult<UserRoleResultDTO>>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public ChangeUserRoleCommandHandler(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<OperationResult<UserRoleResultDTO>> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.ChangeUserRoleDTO.UserId.ToString());
                if (user == null)
                {
                    return OperationResult<UserRoleResultDTO>.Fail("User not found.", nameof(Handle));
                }

                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Contains(request.ChangeUserRoleDTO.NewRole))
                {
                    return OperationResult<UserRoleResultDTO>.Fail($"User already has the role {request.ChangeUserRoleDTO.NewRole}.", nameof(Handle));
                }

                var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeRolesResult.Succeeded)
                {
                    return OperationResult<UserRoleResultDTO>.Fail("Failed to remove current roles.", nameof(Handle));
                }

                var addRoleResult = await _userManager.AddToRoleAsync(user, request.ChangeUserRoleDTO.NewRole);
                if (!addRoleResult.Succeeded)
                {
                    return OperationResult<UserRoleResultDTO>.Fail("Failed to add new role.", nameof(Handle));
                }

                var result = new UserRoleResultDTO
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    CurrentRole = request.ChangeUserRoleDTO.NewRole
                };

                return OperationResult<UserRoleResultDTO>.Success(result);
            }
            catch (Exception ex)
            {
                return OperationResult<UserRoleResultDTO>.Fail($"An unexpected error occurred: {ex.Message}", nameof(Handle));
            }
        }
    }
}
