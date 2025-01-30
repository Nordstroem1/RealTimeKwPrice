using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.DTO.User
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "username is required.")]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; }
        [Phone]
        [Required]
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        [Required]
        public string Location { get; set; }
        public CreateUserDto(string userName, string email, string password, string phoneNumber, string location)
        {
            UserName = userName;
            Email = email;
            Password = password;
            PhoneNumber = phoneNumber;
            Location = location;
        }
    }
}
