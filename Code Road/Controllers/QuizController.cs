using Code_Road.Dto.Account;
using Code_Road.Dto.Quizzes;
using Code_Road.Models;
using Code_Road.Services.QuizService;
using Microsoft.AspNetCore.Mvc;

namespace Code_Road.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpGet("GetAllQuizzes")]
        public async Task<IActionResult> GetAllQuizzes()
        {
            List<Quiz> quizzes = await _quizService.GetAllQuizzes();
            if (quizzes is not null)
                return Ok(new { Message = $"Ther is {quizzes.Count} quizzes", Quiz = quizzes });

            return BadRequest("Ther is no quizzes");
        }

        [HttpPost("GetQuizById/{QuizId:int}")]
        public async Task<IActionResult> GetQuizById(int QuizId)
        {
            if (ModelState.IsValid)
            {
                GetQuizDetailsDto quizDetails = await _quizService.GetQuizWithId(QuizId);
                if (quizDetails.State.Flag)
                    return Ok(quizDetails);
                return BadRequest(quizDetails.State.Message);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("CreateQuiz")]
        public async Task<IActionResult> CreateQuiz(AddQuizDto model)
        {
            if (ModelState.IsValid)
            {
                return Ok(await _quizService.AddQuiz(model));
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("DeleteQuiz/{QuizId:int}")]
        public async Task<IActionResult> DeleteQuiz(int QuizId)
        {
            if (ModelState.IsValid)
            {
                StateDto status = await _quizService.DeleteQuiz(QuizId);
                if (status.Flag)
                    return Ok(status.Message);
                return BadRequest(status.Message);
            }
            return BadRequest(ModelState);
        }
    }
}
