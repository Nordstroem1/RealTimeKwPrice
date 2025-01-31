using Application.DTO.Login;
using Application.DTO.User;
using Domain.Models;
using MediatR;

namespace Application.Users.Queries.LogInUser
{
    public class LoginUserQuery : IRequest<OperationResult<(User, string)>>
    {
        public LoginDto LoginDto { get; }
        public LoginUserQuery(LoginDto loginDto)
        {
            LoginDto = loginDto;
        }
    }
}
