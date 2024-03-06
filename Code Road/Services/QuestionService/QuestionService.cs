using Code_Road.Dto.Account;
using Code_Road.Dto.Questions;
using Code_Road.Models;
using Microsoft.EntityFrameworkCore;

namespace Code_Road.Services.QuestionService
{
    public class QuestionService : IQuestionService
    {
        private readonly AppDbContext _context;

        public QuestionService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<GetQuestionDetailsDto> GetAllQuestions()
        {
            GetQuestionDetailsDto QuestionDetails = new GetQuestionDetailsDto();
            List<Question> questions = await _context.Questions.ToListAsync();
            StateDto state = new StateDto();
            if (questions.Count > 0) // if we found questions for this quiz 
            {
                state.Flag = true;
                state.Message = $"There Is {questions.Count} Questions";
                QuestionDetails.Status = state;
                QuestionDetails.Questions = questions;
                return QuestionDetails;
            }
            // if not found questions for quiz id
            state.Flag = false;
            state.Message = "Ther Is No Questions";
            QuestionDetails.Status = state;
            return QuestionDetails;
        }
        public async Task<GetQuestionDetailsDto> GetQuizQuestions(int QuizId)
        {
            GetQuestionDetailsDto QuestionDetails = new GetQuestionDetailsDto();
            StateDto state = new StateDto();
            List<Question> questions = await _context.Questions.Where(q => q.QuizId == QuizId).ToListAsync();
            if (questions.Count > 0) // if we found questions for this quiz 
            {
                state.Flag = true;
                state.Message = $"There Is {questions.Count} Questions For This Quiz";
                QuestionDetails.Status = state;
                QuestionDetails.Questions = questions;
                return QuestionDetails;
            }
            // if not found questions for quiz id
            state.Flag = false;
            state.Message = "Not Found Questions For This Quizzes Id";
            QuestionDetails.Status = state;
            return QuestionDetails;
        }
        public async Task<StateDto> AddQuestion(int QuizId, AddQuestionDto Model)
        {
            Quiz? quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.Id == QuizId);
            if (quiz is not null)
            {
                await _context.Questions.AddAsync(new Question
                {
                    QuestionContent = Model.QuestionContent,
                    Option1 = Model.Option1,
                    Option2 = Model.Option2,
                    Option3 = Model.Option3,
                    Option4 = Model.Option4,
                    CorrectAnswer = Model.CorrectAnswer,
                    Degree = Model.Degree,
                    QuizId = QuizId
                });
                await _context.SaveChangesAsync();
                return new StateDto { Flag = true, Message = "Added Successfully" };
            }
            return new StateDto { Flag = false, Message = "This Quizzes Not Found!!" };
        }
        public async Task<StateDto> UpdateQuestion(int QuestionId, AddQuestionDto Model)
        {
            Question? question = await _context.Questions.FirstOrDefaultAsync(q => q.Id == QuestionId);
            if (question is not null)
            {
                // update question
                question.QuestionContent = Model.QuestionContent;
                question.Option1 = Model.Option1;
                question.Option2 = Model.Option2;
                question.Option3 = Model.Option3;
                question.Option4 = Model.Option4;
                question.CorrectAnswer = Model.CorrectAnswer;
                question.Degree = Model.Degree;
                // save changes
                await _context.SaveChangesAsync();

                return new StateDto { Flag = true, Message = "Update Question Successfully" };
            }
            return new StateDto { Flag = true, Message = $"This Question Not Found" };
        }
        public async Task<StateDto> DeleteQuestion(int QuestionId)
        {
            Question? question = await _context.Questions.FirstOrDefaultAsync(q => q.Id == QuestionId);
            if (question is not null)
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();

                return new StateDto { Flag = true, Message = $"Question With Id {QuestionId} Deleted Successfully" };
            }
            return new StateDto { Flag = false, Message = $"There Is No Question With Id {QuestionId}" };
        }
        public async Task<StateDto> DeleteAllQuizQuestions(int QuizId)
        {
            List<Question> questions = await _context.Questions.Where(q => q.QuizId == QuizId).ToListAsync();
            if (questions.Count > 0)
            {
                _context.Questions.RemoveRange(questions);
                await _context.SaveChangesAsync();
                return new StateDto { Flag = true, Message = "All Questions Deleted Successfully" };
            }
            return new StateDto { Flag = false, Message = "There Is No Questions For This Quiz Id" };
        }
    }
}
