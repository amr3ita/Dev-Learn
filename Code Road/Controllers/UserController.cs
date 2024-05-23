using Code_Road.Dto.Account;
using Code_Road.Dto.User;
using Code_Road.Services.UserService;
using Microsoft.AspNetCore.Mvc;

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

        // get all followers for any user
        [HttpGet("GetAllFollowers")]
        public async Task<IActionResult> GetAllFollowers(string id)
        {

            FollowersDto followeres = await _userService.GetAllFollowers(id);
            if (!followeres.State.Flag)
                return BadRequest(followeres);
            return Ok(followeres);
        }

        // get the following list for the user
        [HttpGet("GetAllFollowing")]
        public async Task<IActionResult> GetAllFollowing(string id)
        {

            FollowingDto? followeres = await _userService.GetAllFollowing(id);
            if (followeres is null)
                return BadRequest("SomeThing went wrong");
            if (followeres.State is not null)
            {
                if (!followeres.State.Flag)
                    return BadRequest(followeres);
            }
            return Ok(followeres);
        }

        // make follow to another user
        [HttpPost("Follow")]
        public async Task<IActionResult> Follow(string followerId, string followingId)
        {

            StateDto follower = await _userService.Follow(followerId, followingId);
            if (!follower.Flag)
                return BadRequest(follower);
            return Ok(follower);
        }

        // unfollow any user
        [HttpPost("UnFollow")]
        public async Task<IActionResult> UnFollow(string followerId, string followingId)
        {

            StateDto follower = await _userService.UnFollow(followerId, followingId);

            if (!follower.Flag)
                return BadRequest(follower);
            return Ok(follower);
        }

    }
}
