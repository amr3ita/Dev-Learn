using Code_Road.Dto.Account;
using Code_Road.Dto.Quizzes;
using Code_Road.Models;

namespace Code_Road.Services.QuizService
{
    public interface IQuizService
    {
        public Task<List<Quiz>> GetAllQuizzes();
        public Task<GetQuizDetailsDto> GetQuizWithId(int QuizId);
        public Task<StateDto> AddQuiz(AddQuizDto Model);
        public Task<StateDto> DeleteQuiz(int QuizId);
    }
}
