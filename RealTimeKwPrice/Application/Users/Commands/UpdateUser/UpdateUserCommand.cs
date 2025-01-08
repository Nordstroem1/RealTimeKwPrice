using Domain.Models;
using MediatR;

namespace Application.Commands
{
    public class UpdateUserCommand : IRequest<User>
    {
        public Guid Id { get; set; }
        public User User { get; set; }

        public UpdateUserCommand(Guid id, User user)
        {
            Id = id;
            User = user;
        }
    }
}