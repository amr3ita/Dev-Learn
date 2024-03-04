using Code_Road.Dto.Account;
using Code_Road.Models;

namespace Code_Road.Dto.Questions
{
    public class GetQuestionDetailsDto
    {
        public StateDto Status { get; set; }
        public List<Question>? Questions { get; set; }
    }
}
