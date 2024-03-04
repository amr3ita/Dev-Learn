using Code_Road.Dto.Account;
using Code_Road.Dto.Questions;

namespace Code_Road.Services.QuestionService
{
    public interface IQuestionService
    {
        public Task<GetQuestionDetailsDto> GetAllQuestions();
        public Task<GetQuestionDetailsDto> GetQuizQuestions(int QuizId);
        public Task<StateDto> AddQuestion(int QuizId, AddQuestionDto Model);
        public Task<StateDto> UpdateQuestion(int QuestionId, AddQuestionDto Model);
        public Task<StateDto> DeleteQuestion(int QuestionId);
        public Task<StateDto> DeleteAllQuizQuestions(int QuizId);
    }
}
