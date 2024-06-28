using Microsoft.AspNetCore.Identity;

namespace Code_Road.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime ActiceDay { get; set; }
        public int OnlineDays { get; set; }
        public List<ApplicationUser>? Followers { get; set; }
        public List<ApplicationUser>? Following { get; set; }
        public List<Lesson>? FinishedTopics { get; set; }
        public List<Post>? Posts { get; set; }
        public List<PostVote>? PostVotes { get; set; }
        public Image Image { get; set; }

    }
}
