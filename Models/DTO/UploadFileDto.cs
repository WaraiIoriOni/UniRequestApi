namespace UniRequestAPI.Models.DTO
{
    public class UploadFileDto
    {
        public string ObjectType { get; set; } 
        public int ObjectId { get; set; }
        public string FileBase64 { get; set; } 
        public string FileName { get; set; }
    }
}
