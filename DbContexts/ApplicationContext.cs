using Microsoft.EntityFrameworkCore;
using UniRequestAPI.Models.Comments;
using UniRequestAPI.Models.People;
using UniRequestAPI.Models.Users;

namespace UniRequestAPI.DbContexts
{
    public class ApplicationContext: DbContext
    {
        public DbSet<User> Users { get; set; } = null;
        public DbSet<Person> People { get; set; } = null;
        public DbSet<Request> Requests { get; set; } = null;
        public DbSet<Appeal> Appeals { get; set; } = null;
        public DbSet<Question> Questions { get; set; } = null;
        public DbSet<Suggestion> Suggestions { get; set; } = null;
        public DbSet<Complaint> Complaints { get; set; } = null;
        public DbSet<UserFile> UserFiles { get; set; } = null;
        public DbSet<RegistrationRequest> RegistrationRequests { get; set; } = null;
        public DbSet<MessageRequest> MessageRequests { get; set; } = null;

        public ApplicationContext()
        {
            //Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=RequestAPIDB;Username=postgres;Password=Dobriyna");
        }
    }
}
