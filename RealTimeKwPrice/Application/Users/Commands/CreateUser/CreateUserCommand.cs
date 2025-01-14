using Application.DTO.User;
using Domain.Models;
using MediatR;

namespace Application.Users.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<OperationResult<User>>
    {
        public CreateUserCommand(CreateUserDto userDto)
        {
            UserDto = userDto;
        }
        public CreateUserDto UserDto { get; }
    }
}
