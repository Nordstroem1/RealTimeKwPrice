using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class User : IdentityUser<Guid>
    {
        [Required]
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        [Required]
        public RoleEnums.Roles Role { get; set; }
        [Required]
        public string Location { get; set; }
        public List<KiloWattPrice> Price { get; set; }
        public User(Guid userId, string userName, string email, string phoneNumber, RoleEnums.Roles role, string location)
        {
            Id = userId;
            UserName = userName;
            Email = email;
            PhoneNumber = phoneNumber;
            Location = location;
            Role = role;
            LockoutEnabled = true;
            LockoutEnd = null;
        }   
        public User()
        {
        }
    }
}
