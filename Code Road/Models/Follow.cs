using System.ComponentModel.DataAnnotations.Schema;

namespace Code_Road.Models
{
    public class Follow
    {
        [ForeignKey(nameof(Follower))]
        public string FollowerId { get; set; }
        [ForeignKey(nameof(Following))]
        public string FollowingId { get; set; }
        public virtual ApplicationUser? Follower { get; set; }
        public virtual ApplicationUser? Following { get; set; }
    }
}
