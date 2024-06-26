using Code_Road.Dto.Account;
using Code_Road.Dto.Comment;
using Code_Road.Services.CommentService;
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

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
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
        [HttpDelete("UserDeleteHisComment")]
        [Authorize]
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
        [HttpPost("UserAddThisCooment")]
        [Authorize]
        public async Task<IActionResult> AddComment(int postId, EditDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            string userId = await getLogginUserId();
            StateDto state = await _commentService.AddComment(postId, userId, model);
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
