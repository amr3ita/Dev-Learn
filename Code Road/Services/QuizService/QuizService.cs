using Code_Road.Dto.Account;
using Code_Road.Dto.Quizzes;
using Code_Road.Models;
using Code_Road.Services.QuestionService;
using Microsoft.EntityFrameworkCore;

namespace Code_Road.Services.QuizService
{
    public class QuizService : IQuizService
    {
        private readonly AppDbContext _context;
        private readonly IQuestionService _question;

        public QuizService(AppDbContext context, IQuestionService question)
        {
            _context = context;
            _question = question;
        }
        public async Task<List<Quiz>> GetAllQuizzes()
        {
            GetQuizDetailsDto quizDetails = new GetQuizDetailsDto();
            List<Quiz> quizzes = await _context.Quizzes.Include(q => q.Questions).ToListAsync();
            if (quizzes.Count > 0)
                return quizzes;

            return null; // if not found quizzes
        }
        public async Task<GetQuizDetailsDto> GetQuizWithId(int QuizId)
        {
            GetQuizDetailsDto quizDetails = new GetQuizDetailsDto();
            StateDto status = new StateDto();
            Quiz quiz = await _context.Quizzes.Include(q => q.Questions).FirstOrDefaultAsync(q => q.Id == QuizId);
            if (quiz is not null)
            {
                status.Flag = true;
                status.Message = $"Successfully";
                quizDetails.State = status; // configure state

                quizDetails.Quiz = quiz; // configre quiz
                return quizDetails;
            }
            status.Flag = false;
            status.Message = $"There is no quiz with id {QuizId}";
            quizDetails.State = status;

            return quizDetails;
        }
        public async Task<StateDto> AddQuiz(AddQuizDto Model)
        {
            Quiz newQuiz = new Quiz { LessonId = Model.LessonId, TotalDegree = 0 };
            await _context.Quizzes.AddAsync(newQuiz);
            await _context.SaveChangesAsync();

            foreach (var question in Model.Questions)
            {
                await _question.AddQuestion(newQuiz.Id, question);
                newQuiz.TotalDegree += question.Degree;
            }
            _context.Quizzes.Update(newQuiz);
            await _context.SaveChangesAsync();

            return new StateDto { Flag = true, Message = "Quiz Added Successfully" };
        }
        public async Task<StateDto> DeleteQuiz(int QuizId)
        {
            Quiz? quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.Id == QuizId);
            if (quiz is not null)
            {
                StateDto status = await _question.DeleteAllQuizQuestions(QuizId);
                if (status.Flag)
                {
                    _context.Quizzes.Remove(quiz);
                    await _context.SaveChangesAsync();
                    return new StateDto { Flag = true, Message = "Quiz Deleted Successfully And All Linked Questions" };
                }
                _context.Quizzes.Remove(quiz);
                await _context.SaveChangesAsync();
                return new StateDto { Flag = true, Message = "Quiz Deleted Successfully But There Is No Questions For This Quiz" };
            }
            return new StateDto { Flag = false, Message = $"There Is No Quiz With Id {QuizId}" };
        }
    }
}
