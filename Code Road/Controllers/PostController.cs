using Code_Road.Dto.Account;
using Code_Road.Dto.Post;
using Code_Road.Services.PostService;
using Code_Road.Services.PostService.AuthService;
using Code_Road.Services.VotesService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Code_Road.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IVoteService _voteService;
        private readonly IAuthService _authService;

        public PostController(IAuthService authService, IPostService postService, IVoteService voteService)
        {
            _postService = postService;
            _voteService = voteService;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _postService.GetAllAsync();

            if (posts is not null)
                return Ok(posts);

            return Ok(new StateDto { Flag = false, Message = "No posts found" });

        }

        [HttpGet("userPosts/{userId}")]
        public async Task<IActionResult> GetAllByUserId(string userId)
        {

            var posts = await _postService.GetAllByUserIdAsync(userId);
            if (posts.Count > 0)
            {
                if (!posts[0].Status.Flag) return Ok(posts[0].Status.Message);
                return Ok(posts);
            }

            return NotFound(new StateDto { Flag = false, Message = "No posts found for the specified user" });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var post = await _postService.GetByIdAsync(id);
            if (post.State.Flag)
                return Ok(post);
            return Ok(post.State.Message);
        }

        [Authorize]
        [HttpPost("CreatePost")]
        public async Task<IActionResult> CreatePost([FromForm] AddPostDto postModel)
        {
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser is not null)
            {
                var post = await _postService.AddPostAsync(currentUser, postModel);

                if (post.Status.Flag)
                    return Ok(post);
                return Ok(post.Status.Message);
            }
            return BadRequest("Login First");
        }

        [Authorize]
        [HttpPut("{post_id:int}")]
        public async Task<IActionResult> Update([FromRoute] int post_id, [FromForm] UpdatePostDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else
            {
                var currentUser = await _authService.GetCurrentUserAsync();
                var returned = await _postService.UpdatePostAsync(currentUser, post_id, model);
                if (!returned.Status.Flag)
                    return Ok(returned.Status.Message);
                return Ok(returned);
            }
        }

        //[Authorize]
        [HttpGet("GetUpVotes/{postId:int}")]
        public async Task<IActionResult> GetUpVotes(int postId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var votes = await _postService.GetUpVotes(postId);
            if (!votes[0].State.Flag)
                return Ok(votes[0].State.Message);
            return Ok(votes);

        }

        // [Authorize]
        [HttpGet("GetDownVotes/{postId:int}")]
        public async Task<IActionResult> GetDownVotes(int postId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var votes = await _postService.GetDownVotes(postId);
            if (!votes[0].State.Flag)
                return Ok(votes[0].State.Message);
            return Ok(votes);

        }

        [Authorize]
        [HttpPost("Vote")]
        public async Task<IActionResult> Vote(int postId, int vote)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            StateDto state = await _voteService.PostVote(postId, vote);
            if (!state.Flag)
                return Ok(state.Message);
            return Ok(state);
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> RemovePost(int id)
        {
            var currentUser = await _authService.GetCurrentUserAsync();
            var state = await _postService.DeletePostAsync(currentUser, id);
            if (!state.Flag)
            {
                return Ok(state.Message);
            }
            return Ok(state.Message);
        }


    }
}
