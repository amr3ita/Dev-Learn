using Code_Road.Dto.Questions;

namespace Code_Road.Dto.Quizzes
{
    public class AddQuizDto
    {
        public int LessonId { get; set; }
        public List<AddQuestionDto>? Questions { get; set; }
    }
}
