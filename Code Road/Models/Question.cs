using System.ComponentModel.DataAnnotations.Schema;

namespace Code_Road.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionContent { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string? Option3 { get; set; }
        public string? Option4 { get; set; }

        public string CorrectAnswer { get; set; }
        public int MinDegree { get; set; }


        [ForeignKey(nameof(Quiz))]
        public int QuizId { get; set; }
        public virtual Quiz Quiz { get; set; }
    }
}
