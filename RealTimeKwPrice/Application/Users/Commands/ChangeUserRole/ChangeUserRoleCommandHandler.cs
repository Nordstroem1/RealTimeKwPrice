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
    public class ChangeUserRoleCommandHandler : IRequest<OperationResult<User>>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public ChangeUserRoleCommandHandler(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<UserRoleResultDTO> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.ChangeUserRoleDTO.UserId.ToString());
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Contains(request.ChangeUserRoleDTO.NewRole))
            {
                throw new ArgumentException($"User already has the role {request.ChangeUserRoleDTO.NewRole}.");
            }

            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, request.ChangeUserRoleDTO.NewRole);

            return new UserRoleResultDTO
            {
                UserId = user.Id,
                UserName = user.UserName,
                CurrentRole = request.ChangeUserRoleDTO.NewRole
            };
        }
    }
}
