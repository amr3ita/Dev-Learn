using Code_Road.Dto.Account;
using Code_Road.Dto.User;
using Code_Road.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Code_Road.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        #region Follow
        [Authorize]
        [HttpGet("GetAllFollowers")]
        public async Task<IActionResult> GetAllFollowers()
        {
            string userId = await getLogginUserId();
            FollowersDto followeres = await _userService.GetAllFollowers(userId);
            if (followeres.State is not null)
            {
                if (!followeres.State.Flag)
                    return BadRequest(followeres);
            }
            return Ok(followeres);
        }
        [Authorize]
        [HttpGet("GetAllFollowing")]
        public async Task<IActionResult> GetAllFollowing()
        {
            string userId = await getLogginUserId();
            FollowingDto? followeres = await _userService.GetAllFollowing(userId);
            if (followeres is null)
                return BadRequest("SomeThing went wrong");
            if (followeres.State is not null)
            {
                if (!followeres.State.Flag)
                    return BadRequest(followeres);
            }
            return Ok(followeres);
        }

        [Authorize]
        [HttpPost("Follow")]
        public async Task<IActionResult> Follow(string followingId)
        {
            string followerId = await getLogginUserId();
            StateDto follower = await _userService.Follow(followerId, followingId);
            if (!follower.Flag)
                return BadRequest(follower);
            return Ok(follower);
        }
        [Authorize]
        [HttpPost("UnFollow")]
        public async Task<IActionResult> UnFollow(string followingId)
        {
            string followerId = await getLogginUserId();
            StateDto follower = await _userService.UnFollow(followerId, followingId);

            if (!follower.Flag)
                return BadRequest(follower);
            return Ok(follower);
        }
        #endregion
        [Authorize]
        [HttpGet("GetFinishedLessonsForSpecificUser")]
        public async Task<IActionResult> GetFinishedLessonsForSpecificUser()
        {
            string userId = await getLogginUserId();
            FinishedLessonsDto finishedLessons = await _userService.GetFinishedLessonsForSpecificUser(userId);
            if (finishedLessons.State is not null)
            {
                if (!finishedLessons.State.Flag)
                    return BadRequest(finishedLessons);
            }
            return Ok(finishedLessons);
        }
        [Authorize]
        [HttpPost("FinishNewLesson")]
        public async Task<IActionResult> FinishLesson(int lessonId, int degree)
        {
            string userId = await getLogginUserId();
            StateDto state = await _userService.FinishLesson(userId, lessonId, degree);
            return Ok(state);
        }
        private async Task<string> getLogginUserId()
        {
            string id = HttpContext.User.FindFirstValue("uid") ?? "NA";
            return id;
        }
    }
}
