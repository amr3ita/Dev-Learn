using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Code_Road.Models
{
    public class Question
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string QuestionContent { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string? Option3 { get; set; }
        public string? Option4 { get; set; }
        public string CorrectAnswer { get; set; }
        public int Degree { get; set; }

        [ForeignKey(nameof(Quiz))]
        public int QuizId { get; set; }
        [JsonIgnore]
        public virtual Quiz Quiz { get; set; }
    }
}
