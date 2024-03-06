using Code_Road.Dto.Account;
using Code_Road.Models;

namespace Code_Road.Dto.Quizzes
{
    public class GetQuizDetailsDto
    {
        public StateDto State { get; set; }
        public Quiz Quiz { get; set; }
    }
}
