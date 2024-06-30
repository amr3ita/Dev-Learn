using Code_Road.Dto.Account;
using Code_Road.Dto.Comment;
using Code_Road.Services.CommentService;
using Code_Road.Services.VotesService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Code_Road.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IVoteService _voteService;

        public CommentController(ICommentService commentService, IVoteService voteService)
        {
            _commentService = commentService;
            _voteService = voteService;
        }
        [HttpGet("CommentsForPost/{postId:int}")]
        public async Task<IActionResult> GetComments(int postId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            List<CommentDto> comments = await _commentService.GetComments(postId);
            foreach (CommentDto comment in comments)
            {
                if (!comment.State.Flag)
                    return BadRequest(comment.State.Message);
            }
            return Ok(comments);
        }
        [HttpPatch("edit")]
        [Authorize]
        public async Task<IActionResult> EditComment(int commentId, EditDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            string userId = await getLogginUserId();
            CommentDto comment = await _commentService.EditComment(commentId, userId, model);
            if (!comment.State.Flag)
                return BadRequest(comment.State.Message);
            return Ok(comment);
        }
        [Authorize]
        [HttpDelete("DeleteComment")]
        public async Task<IActionResult> DeleteComment(int commentId, int postId)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            string userId = await getLogginUserId();
            StateDto state = await _commentService.DeleteComment(commentId, postId, userId);
            if (!state.Flag)
                return BadRequest(state.Message);
            return Ok(state.Message);
        }
        [Authorize]
        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment(int postId, string content)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            StateDto state = await _commentService.AddComment(postId, content);
            if (!state.Flag)
                return BadRequest(state.Message);
            return Ok(state);
        }
        //[Authorize]
        [HttpGet("GetUpVotes/{commentId:int}")]
        public async Task<IActionResult> GetUpVotes(int commentId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var votes = await _commentService.GetUpVotes(commentId);
            if (!votes[0].State.Flag)
                return BadRequest(votes[0].State.Message);
            return Ok(votes);

        }
        // [Authorize]
        [HttpGet("GetDownVotes/{commentId:int}")]
        public async Task<IActionResult> GetDownVotes(int commentId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var votes = await _commentService.GetDownVotes(commentId);
            if (!votes[0].State.Flag)
                return BadRequest(votes[0].State.Message);
            return Ok(votes);

        }
        [Authorize]
        [HttpPost("Vote")]
        public async Task<IActionResult> Vote(int commentId, int vote)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            StateDto state = await _voteService.CommentVote(commentId, vote);
            if (!state.Flag)
                return BadRequest(state.Message);
            return Ok(state);
        }
        private async Task<string> getLogginUserId()
        {

            string id = HttpContext.User.FindFirstValue("uid") ?? "NA";
            return id;
        }
    }
}
