using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniRequestAPI.Models.Users
{
    public class RegistrationRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string? Email { get; set; } = null;
        public string PhoneNumber { get; set; }
        public string? Department { get; set; } = null;
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
