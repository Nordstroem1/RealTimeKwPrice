using MediatR;
using Domain.Interfaces;
using Domain.Models;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Application.Commands
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, OperationResult<User>>
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public UpdateUserCommandHandler(IGenericRepository<User> userRepository,
            RoleManager<IdentityRole<Guid>> roleManager, UserManager<User> usermanager)
        {
            _userRepository = userRepository;
            _roleManager = roleManager;
            _userManager = usermanager;
        }

        public async Task<OperationResult<User>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByIdAsync(request.Id);
            if (existingUser == null)
            {
                return OperationResult<User>.Fail("User not found", nameof(UpdateUserCommandHandler));
            }

            existingUser.UserName = request.User.UserName;
            existingUser.Email = request.User.Email;
            existingUser.PhoneNumber = request.User.PhoneNumber;
            existingUser.PasswordHash = _userManager.PasswordHasher.HashPassword(existingUser, request.User.PasswordHash);
            existingUser.Location = request.User.Location;

            var currentRoles = await _userManager.GetRolesAsync(existingUser);
            var newRole = request.User.Role.ToString();
            if(!currentRoles.Contains(newRole))
            {
                await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);
                await _userManager.AddToRoleAsync(existingUser, newRole);
            }

            var result = await _userManager.UpdateAsync(existingUser);
            if (!result.Succeeded)
            {
                return OperationResult<User>.Fail("Failed to update user", nameof(UpdateUserCommandHandler));
            }

            return OperationResult<User>.Success(existingUser);
        }
    }
}