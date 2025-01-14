using Application.DTO.Role;
using Application.DTO.Role.SuperAdmin;
using Domain.Models;
using MediatR;

namespace Application.Users.Commands.ChangeAdminRoleAsSuperAdmin
{
    public class ChangeRoleAsSuperAdminCommand : IRequest<OperationResult<ChangeRoleAsSuperAdminDto>>
    {
        public ChangeRoleAsSuperAdminDto RoleDTO { get; set; }
        public ChangeRoleAsSuperAdminCommand(ChangeRoleAsSuperAdminDto roleDTO)
        {
            RoleDTO = roleDTO;
        }
    }
}
