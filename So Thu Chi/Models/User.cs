using System.ComponentModel.DataAnnotations;

namespace So_Thu_Chi.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; } 

        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
    }
}
