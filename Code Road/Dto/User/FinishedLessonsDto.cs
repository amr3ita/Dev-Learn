using Code_Road.Dto.Account;

namespace Code_Road.Dto.User
{
    public class FinishedLessonsDto
    {
        public StateDto State { get; set; }
        public int Count { get; set; }
        public List<FinishedLessonDetailsDto>? FinishedLessons { get; set; }
    }
}
