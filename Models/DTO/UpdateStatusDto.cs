namespace UniRequestAPI.Models.DTO
{
    public class UpdateStatusDto
    {
        public int Id { get; set; }
        public string Type { get; set; } 
        public string Status { get; set; }
        public string? ResponseText { get; set; }
    }
}
