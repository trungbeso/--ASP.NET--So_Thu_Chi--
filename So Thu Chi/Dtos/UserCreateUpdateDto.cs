using System.ComponentModel.DataAnnotations;

namespace So_Thu_Chi.Dtos
{
    public class UserCreateUpdateDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string PhoneNumber { get; set; } 

        [EmailAddress]
        public string Email { get; set; }

        public string Address { get; set; }
        public string Role { get; set; }
    }
}
