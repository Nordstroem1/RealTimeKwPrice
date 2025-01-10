using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class User : IdentityUser<Guid>
    {
        [Required]
        public DateTime CreatedAt { get; set; } 
        [Required]
        public RoleEnums.Roles Role { get; set; }
        [Required]
        public string Location { get; set; }
        public List<ElectricityPrice> PriceList { get; set; }
        public User(Guid userId, string userName, string email, string phoneNumber, RoleEnums.Roles role, string location, DateTime createdAt)
        {
            Id = userId;
            UserName = userName;
            Email = email;
            PhoneNumber = phoneNumber;
            Location = location;
            Role = role;
            CreatedAt = createdAt;
            LockoutEnabled = true;
            LockoutEnd = null;
        }   
        public User()
        {
        }
    }
}
