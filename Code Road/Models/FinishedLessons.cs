using System.ComponentModel.DataAnnotations.Schema;

namespace Code_Road.Models
{
    public class FinishedLessons
    {
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        [ForeignKey(nameof(Lesson))]
        public int LessonId { get; set; }
        public int Degree { get; set; }
        public virtual Lesson Lesson { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
