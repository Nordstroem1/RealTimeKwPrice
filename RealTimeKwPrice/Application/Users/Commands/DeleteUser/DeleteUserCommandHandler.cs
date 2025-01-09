using MediatR;
using Domain.Models;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Users.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, OperationResult<User>>
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly UserManager<User> _userManager;

        public DeleteUserCommandHandler(IGenericRepository<User> userRepository, RoleManager<IdentityRole<Guid>> roleManager, UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<OperationResult<User>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userManager.FindByIdAsync(request.Id.ToString());
            if (existingUser == null)
            {
                return OperationResult<User>.Fail("User not found", nameof(DeleteUserCommandHandler));
            }

            var result = await _userManager.DeleteAsync(existingUser);
            if (!result.Succeeded)
            {
                return OperationResult<User>.Fail("Failed to delete user", nameof(DeleteUserCommandHandler));
            }

            return OperationResult<User>.Success(existingUser);
        }
    }
}