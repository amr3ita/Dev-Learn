using Code_Road.Models;

namespace Code_Road.Dto.Quizzes
{
    public class GetQuizDetailsDto
    {
        public int QuizId { get; set; }
        public int? TotalDegree { get; set; }
        public string LessonName { get; set; }
        public List<Question>? Questions { get; set; }
    }
}
