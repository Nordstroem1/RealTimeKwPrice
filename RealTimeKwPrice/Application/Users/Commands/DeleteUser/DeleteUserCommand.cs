using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Commands.DeleteUser
{
    public class DeleteUserCommand : IRequest<OperationResult<User>>
    {
        public Guid Id { get; set; }

        public User User { get; set; }

        public DeleteUserCommand(Guid id, User user)
        {
            Id = id;
            User = user;
        }
    }
}
