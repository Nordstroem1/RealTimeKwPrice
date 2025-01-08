using MediatR;
using Domain.Interfaces;
using Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, User>
    {
        private readonly IGenericRepository<User> _userRepository;

        public UpdateUserCommandHandler(IGenericRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByIdAsync(request.Id);
            if (existingUser == null)
            {
                return null;
            }

            existingUser.UserName = request.User.UserName;
            existingUser.Email = request.User.Email;
            existingUser.PhoneNumber = request.User.PhoneNumber;
            existingUser.PasswordHash = request.User.PasswordHash;
            existingUser.Role = request.User.Role;
            existingUser.Location = request.User.Location;

            return await _userRepository.UpdateAsync(existingUser);
        }
    }
}