using System.ComponentModel.DataAnnotations.Schema;

namespace Code_Road.Models
{
    public class Post
    {
        public int Id { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public string Content { get; set; }
        public int Up { get; set; }
        public int Down { get; set; }
        public DateTime Date { get; set; }
        public virtual List<Image> Images { get; set; }
        public virtual List<Comment> comments { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual List<PostVote>? PostVotes { get; set; }
    }
}
