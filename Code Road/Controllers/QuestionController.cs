using Code_Road.Dto.Account;
using Code_Road.Dto.Questions;
using Code_Road.Services.QuestionService;
using Microsoft.AspNetCore.Mvc;

namespace Code_Road.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }
        [HttpGet("GetAllQuestions")]
        public async Task<IActionResult> GetAllQuestions()
        {
            GetQuestionDetailsDto QuestionDetails = await _questionService.GetAllQuestions();
            if (QuestionDetails.Status.Flag)
            {
                return Ok(QuestionDetails);
            }
            return BadRequest(QuestionDetails.Status.Message);
        }

        [HttpPost("GetQuizQuestions/{QuizId:int}")]
        public async Task<IActionResult> GetAllQuestions(int QuizId)
        {
            if (ModelState.IsValid)
            {
                GetQuestionDetailsDto QuestionDetails = await _questionService.GetQuizQuestions(QuizId);
                if (QuestionDetails.Status.Flag)
                {
                    return Ok(QuestionDetails);
                }
                return BadRequest(QuestionDetails.Status.Message);

            }
            return BadRequest(ModelState);
        }

        [HttpPost("AddQuestionToQuiz/{QuizId:int}")]
        public async Task<IActionResult> AddQuestionToQuiz(int QuizId, AddQuestionDto model)
        {
            if (ModelState.IsValid)
            {
                StateDto status = await _questionService.AddQuestion(QuizId, model);
                if (status.Flag)
                    return Ok(status.Message);
                return BadRequest(status.Message);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("UpdateQuestion/{QuestionId:int}")]
        public async Task<IActionResult> UpdateQuestion(int QuestionId, AddQuestionDto model)
        {
            if (ModelState.IsValid)
            {
                StateDto status = await _questionService.UpdateQuestion(QuestionId, model);
                if (status.Flag)
                {
                    return Ok(status.Message);
                }
                return BadRequest(status.Message);
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("DeleteQuestion/{QuestionId:int}")]
        public async Task<IActionResult> DeleteQuestion(int QuestionId)
        {
            if (ModelState.IsValid)
            {
                StateDto status = await _questionService.DeleteQuestion(QuestionId);
                if (status.Flag)
                    return Ok(status.Message); // deleted successfully
                return BadRequest(status.Message); // if question not found
            }
            return BadRequest(ModelState);
        }
    }
}
