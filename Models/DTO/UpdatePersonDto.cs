namespace UniRequestAPI.Models.DTO
{
    public class UpdatePersonDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? Department { get; set; }
    }
}
