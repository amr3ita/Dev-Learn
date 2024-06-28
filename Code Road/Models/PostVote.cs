using System.ComponentModel.DataAnnotations.Schema;

namespace Code_Road.Models
{
    public class PostVote
    {
        public int Id { get; set; }
        public bool IsVote { get; set; }
        public string UserName { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        [ForeignKey(nameof(Post))]
        public int PostId { get; set; }

        // Relationships between tables
        public ApplicationUser User { get; set; }
        public Post Post { get; set; }
    }
}
