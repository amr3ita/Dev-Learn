using System.ComponentModel.DataAnnotations.Schema;

namespace Code_Road.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public int? TotalDegree { get; set; }


        [ForeignKey(nameof(Lesson))]
        public int LessonId { get; set; }
        public virtual Lesson Lesson { get; set; }

        public virtual List<Question>? Questions { get; set; }

    }
}
