using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Queries.GetUserById
{
    public class GetUserByIdCommand : IRequest<OperationResult<User>>
    {
        public Guid Id { get; set; }

        public GetUserByIdCommand(Guid id)
        {
            Id = id;
        }
    }
}
