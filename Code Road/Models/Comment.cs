using System.ComponentModel.DataAnnotations.Schema;

namespace Code_Road.Models
{
    public class Comment
    {
        public int Id { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public string Content { get; set; }
        [ForeignKey(nameof(Post))]
        public int PostId { get; set; }
        public int Up { get; set; }
        public int Down { get; set; }
        public DateTime Date { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Post Post { get; set; }
        public virtual List<CommentVote>? CommentVotes { get; set; }

    }
}
