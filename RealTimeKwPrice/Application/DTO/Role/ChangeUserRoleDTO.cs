using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Role
{
    public class ChangeUserRoleDTO
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string NewRole { get; set; }
    }
}
