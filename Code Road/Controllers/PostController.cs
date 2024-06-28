using Code_Road.Dto.Account;
using Code_Road.Dto.Post;
using Code_Road.Services.PostService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Code_Road.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _postService.GetAllAsync();

            if (posts is not null)
                return Ok(posts);

            return NotFound(new StateDto { Flag = false, Message = "No posts found" });

        }
        [HttpGet("userPosts/{userId}")]
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            var posts = await _postService.GetAllByUserIdAsync(userId);

            if (posts is not null)
                return Ok(posts);

            return NotFound(new StateDto { Flag = false, Message = "No posts found for the specified user" });
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var post = await _postService.GetByIdAsync(id);
            if (post.Status.Flag)
                return Ok(post);
            return NotFound(post.Status.Message);
        }
        [HttpPost("CreatePost")]
        public async Task<IActionResult> CreatePost([FromForm] AddPostDto postModel)
        {
            var post = await _postService.AddPostAsync(postModel);

            if (post.Status.Flag)
                return Ok(post);
            return BadRequest(post.Status.Message);
        }
        [HttpPut("{post_id:int}")]
        public async Task<IActionResult> Update([FromRoute] int post_id, [FromForm] UpdatePostDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else
            {
                var returned = await _postService.UpdatePostAsync(post_id, model);
                if (!returned.Status.Flag)
                    return BadRequest(returned.Status.Message);
                return Ok(returned);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> RemovePost(int id, string user_id)
        {
            var state = await _postService.DeletePostAsync(id, user_id);
            if (!state.Flag)
            {
                return BadRequest(state.Message);
            }
            return Ok(state.Message);
        }
        [HttpPost("AddVote/{postId}")]
        public async Task<IActionResult> Vote(int postId, string userId, bool isVote)
        {
            var result = await _postService.VoteAsync(postId, userId, isVote);
            if (result.Flag)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }
        [HttpGet("PostVotes")]
        public async Task<IActionResult> GetAllVotes(int postId)
        {
            var vots= await _postService.GetAllVotesAsync(postId);
            if (vots is null)
            {
                return BadRequest("no vote added yet");
            }
            return Ok(vots);
        }
        #region Up and Down aactions
        /*
        [HttpPut("IncreaseUpvote/{postId}")]
        public async Task<IActionResult> IncreaseUpvote(int postId)
        {
            var result = await _postService.IncreaseUpvoteAsync(postId);
            if (result.Flag)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [HttpPut("IncreaseDownvote/{postId}")]
        public async Task<IActionResult> IncreaseDownvote(int postId)
        {
            var result = await _postService.IncreaseDownvoteAsync(postId);
            if (result.Flag)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [HttpPut("DecreaseUpvote/{postId}")]
        public async Task<IActionResult> DecreaseUpvote(int postId)
        {
            var result = await _postService.DecreaseUpvoteAsync(postId);
            if (result.Flag)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [HttpPut("DecreaseDownvote/{postId}")]
        public async Task<IActionResult> DecreaseDownvote(int postId)
        {
            var result = await _postService.DecreaseDownvoteAsync(postId);
            if (result.Flag)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }
        */
        #endregion


    }
}
