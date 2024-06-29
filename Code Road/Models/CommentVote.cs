using System.ComponentModel.DataAnnotations.Schema;

namespace Code_Road.Models
{
    public class CommentVote
    {
        public int Id { get; set; }
        public int Vote { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string ImageUrl { get; set; }
        [ForeignKey(nameof(Comment))]
        public int CommentId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual Comment? Comment { get; set; }

    }
}
