using Application.DTO;
using Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
