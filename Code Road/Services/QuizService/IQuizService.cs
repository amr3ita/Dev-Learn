using Code_Road.Dto.Account;
using Code_Road.Dto.Quizzes;

namespace Code_Road.Services.QuizService
{
    public interface IQuizService
    {
        public Task<List<GetQuizDetailsDto>> GetAllQuizzes();
        public Task<GetQuizDetailsDto> GetQuizWithId(int QuizId);
        public Task<StateDto> AddQuiz(AddQuizDto Model);
        public Task<StateDto> DeleteQuiz(int QuizId);
    }
}
