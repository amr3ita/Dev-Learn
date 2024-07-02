using Code_Road.Dto.Account;
using Code_Road.Dto.Quizzes;
using Code_Road.Services.PostService.AuthService;
using Code_Road.Services.QuizService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Code_Road.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly IAuthService _authService;

        public QuizController(IQuizService quizService, IAuthService authService)
        {
            _quizService = quizService;
            _authService = authService;
        }

        [Authorize]
        [HttpGet("GetAllQuizzes")]
        public async Task<IActionResult> GetAllQuizzes()
        {
            var quizzes = await _quizService.GetAllQuizzes();
            if (quizzes is not null)
                return Ok(new { Message = $"There is {quizzes.Count} quizzes", Quizzes = quizzes });

            return Ok("Ther is no quizzes");
        }

        [Authorize]
        [HttpGet("GetQuizById/{QuizId:int}")]
        public async Task<IActionResult> GetQuizById(int QuizId)
        {
            if (ModelState.IsValid)
            {
                GetQuizDetailsDto quizDetails = await _quizService.GetQuizWithId(QuizId);
                if (quizDetails is not null)
                    return Ok(quizDetails);
                return Ok($"There is no quiz with id {QuizId}");
            }
            return BadRequest(ModelState);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateQuiz")]
        public async Task<IActionResult> CreateQuiz(AddQuizDto model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _authService.GetCurrentUserAsync();
                if (currentUser.IsAdmin == true)
                {
                    return Ok(await _quizService.AddQuiz(model));
                }
                return Ok("You don't have permission to create quiz");
            }
            return BadRequest(ModelState);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteQuiz/{QuizId:int}")]
        public async Task<IActionResult> DeleteQuiz(int QuizId)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _authService.GetCurrentUserAsync();
                if (currentUser.IsAdmin == true)
                {
                    StateDto status = await _quizService.DeleteQuiz(QuizId);
                    if (status.Flag)
                        return Ok(status.Message);
                    return Ok(status.Message);
                }
                return Ok("You don't have permission to delete this quiz");
            }
            return BadRequest(ModelState);
        }
    }
}
