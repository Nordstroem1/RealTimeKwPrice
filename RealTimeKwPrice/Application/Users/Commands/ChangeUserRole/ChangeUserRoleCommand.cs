using Application.DTO.Role;
using Domain.Models;
using MediatR;

namespace Application.Users.Commands.ChangeUserRole
{
    public class ChangeUserRoleCommand : IRequest<OperationResult<UserRoleResultDTO>>
    {
        public ChangeUserRoleDTO ChangeUserRoleDTO { get; }

        public ChangeUserRoleCommand(ChangeUserRoleDTO dto)
        {
            ChangeUserRoleDTO = dto;
        }
    }
}
