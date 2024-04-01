using Code_Road.Dto.Account;
using Code_Road.Dto.Comment;
using Code_Road.Models;
using Code_Road.Services.CommentService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;

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
        [HttpGet("Comments for Post({postId:int})")]
        public async Task<IActionResult> GetComments(int postId)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            List<CommentDto> comments=await _commentService.GetComments(postId);
             foreach(CommentDto comment in comments)
            {
                if(!comment.State.Flag)
                    return BadRequest(comment.State.Message);
            }
             return Ok(comments);
        }
        [HttpPatch("edit")]
        public async Task<IActionResult> EditComment(int commentId, string userId, EditDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            CommentDto comment=await _commentService.EditComment(commentId, userId,model);
            if (!comment.State.Flag)
                return BadRequest(comment.State.Message);
            return Ok(comment);
        }
        [HttpDelete("user Delete his comment")]

        public async Task<IActionResult> DeleteComment(int commentId, int postId ,string userId) 
        { 

             if (!ModelState.IsValid)
                return BadRequest(ModelState);
        StateDto state = await _commentService.DeleteComment(commentId,postId , userId);
            if (!state.Flag)
                return BadRequest(state.Message);
            return Ok(state.Message);
            }
        [HttpPost("user Add this cooment")]
        public async Task<IActionResult> AddComment(int postId, string userId, EditDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            StateDto state = await _commentService.AddComment(postId, userId , model);
            if (!state.Flag)
                return BadRequest(state.Message);
            return Ok(state);
        }        
    }
}
