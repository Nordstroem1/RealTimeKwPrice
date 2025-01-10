using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class ChangeUserRoleDTO
    {
        public Guid UserId { get; set; }
        public string NewRole { get; set; }
    }
}
