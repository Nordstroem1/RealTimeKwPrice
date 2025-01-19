using Application.DTO.User;
using Domain.Models;
using MediatR;

namespace Application.Users.Queries.LogInUser
{
    public class LoginUserQuery : IRequest<OperationResult<User>>
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public LoginUserQuery(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}
