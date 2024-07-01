using Code_Road.Dto.Account;

namespace Code_Road.Dto.Lesson
{
    public class LessonDto
    {
        public StateDto? State { get; set; }
        public string? Name { get; set; }
        public string? Explanation { get; set; }
        public string? Level { get; set; }
        public List<string>? Img { get; set; }
        public string? Topic { get; set; }
        public int QuizId { get; set; }
    }
}
