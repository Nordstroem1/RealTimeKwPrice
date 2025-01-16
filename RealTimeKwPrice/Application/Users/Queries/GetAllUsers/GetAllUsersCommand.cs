using Domain.Models;
using MediatR;

namespace Application.Users.Queries.GetAllUsers
{
    public class GetAllUsersCommand : IRequest<OperationResult<List<User>>>
    {
    }
}