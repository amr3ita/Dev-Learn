using System.ComponentModel.DataAnnotations.Schema;

namespace Code_Road.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }

        [ForeignKey(nameof(User))]
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }


        [ForeignKey(nameof(Post))]
        public int? PostId { get; set; }
        public virtual Post? Post { get; set; }


        [ForeignKey(nameof(Lesson))]
        public int? LessonId { get; set; }
        public virtual Lesson? Lesson { get; set; }
    }
}
