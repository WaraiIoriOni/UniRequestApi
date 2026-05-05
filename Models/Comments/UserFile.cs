using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniRequestAPI.Models.Comments
{
    public class UserFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }  
        public string FileType { get; set; }     
        public long FileSize { get; set; }       
        public DateTime UploadedAt { get; set; }   
        public string ObjectType { get; set; }
        public int ObjectId { get; set; }
    }
}
