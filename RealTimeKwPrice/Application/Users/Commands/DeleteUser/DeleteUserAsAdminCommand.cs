using Domain.Models;
using MediatR;
using System;

namespace Application.Users.Commands.DeleteUserAsAdmin
{
    public class DeleteUserAsAdminCommand : IRequest<OperationResult<User>>
    {
        public Guid Id { get; set; }

        public DeleteUserAsAdminCommand(Guid id)
        {
            Id = id;
        }
    }
}