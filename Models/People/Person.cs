using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UniRequestAPI.Models.Comments;
using UniRequestAPI.Models.Users;

namespace UniRequestAPI.Models.People
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string? Email { get; set; } = null;
        public string PhoneNumber { get; set; }
        public string? Department { get; set; } = null;
        public int UserId { get; set; }
        public User User { get; set; }
        public List<Request> Requests { get; set; }
        public List<Appeal> Appeals { get; set; }
        public List<Question> Questions { get; set; }
        public List<Suggestion> Suggestions { get; set; }
        public List<Complaint> Complaints { get; set; }
    }
}
