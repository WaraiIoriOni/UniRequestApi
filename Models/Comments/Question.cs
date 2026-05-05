using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UniRequestAPI.Models.People;

namespace UniRequestAPI.Models.Comments
{
    public class Question
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Status { get; set; } = null;
        public string? ResponseText { get; set; } = null;
        public DateTime? RespondedAt { get; set; } = null;
        public int PersonId { get; set; }
        public Person Person { get; set; }
    }
}
