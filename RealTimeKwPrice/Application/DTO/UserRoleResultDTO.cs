using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class UserRoleResultDTO
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string CurrentRole { get; set; }
    }
}
