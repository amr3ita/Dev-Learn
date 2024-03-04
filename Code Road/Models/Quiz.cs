using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Code_Road.Models
{
    public class Quiz
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public int? TotalDegree { get; set; }


        [ForeignKey(nameof(Lesson))]
        public int LessonId { get; set; }
        public virtual Lesson Lesson { get; set; }

        public virtual List<Question>? Questions { get; set; }

    }
}
