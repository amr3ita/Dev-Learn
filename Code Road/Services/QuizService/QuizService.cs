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
        public async Task<List<GetQuizDetailsDto>> GetAllQuizzes()
        {
            List<GetQuizDetailsDto> allQuizzes = new List<GetQuizDetailsDto>();
            List<Quiz> quizzes = await _context.Quizzes.Include(q => q.Questions).ToListAsync();
            foreach (var quiz in quizzes)
            {
                var lessonName = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == quiz.LessonId);
                var questions = await _context.Questions.Where(q => q.QuizId == quiz.Id).ToListAsync();
                allQuizzes.Add(new GetQuizDetailsDto { QuizId = quiz.Id, LessonName = lessonName.Name, TotalDegree = quiz.TotalDegree, Questions = questions });
            }
            return allQuizzes;
        }
        public async Task<GetQuizDetailsDto> GetQuizWithId(int QuizId)
        {
            GetQuizDetailsDto quizDetails = new GetQuizDetailsDto();
            Quiz? quiz = await _context.Quizzes.Include(q => q.Questions).FirstOrDefaultAsync(q => q.Id == QuizId);
            if (quiz is not null)
            {
                var lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == quiz.LessonId);
                quizDetails.QuizId = quiz.Id;
                quizDetails.LessonName = lesson.Name;
                quizDetails.TotalDegree = quiz.TotalDegree;
                quizDetails.Questions = await _context.Questions.Where(q => q.QuizId == quiz.Id).ToListAsync();
                return quizDetails;
            }
            return null;
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
