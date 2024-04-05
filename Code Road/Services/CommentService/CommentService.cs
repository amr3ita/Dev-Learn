using Code_Road.Dto.Account;
using Code_Road.Dto.Comment;
using Code_Road.Models;
using Code_Road.Services.PostService.AuthService;
using Microsoft.EntityFrameworkCore;

namespace Code_Road.Services.CommentService
{
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;

        public CommentService(AppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }
        public async Task<List<CommentDto>> GetComments(int PostId)
        {
            Post? post = await _context.Posts.SingleOrDefaultAsync(p => p.Id == PostId);
            StateDto state = new StateDto();
            List<CommentDto> commentDtos = new List<CommentDto>();
            if (post is null)
            {
                state.Flag = false;
                state.Message = "this Post not found";
                commentDtos.Add(new CommentDto() { State = state });
                return commentDtos;

            }
            List<Comment> comments = await _context.Comments.Where(t => t.PostId == PostId).ToListAsync();
            if (comments.Count <= 0)
            {
                state.Flag = false;
                state.Message = "there is no comments";
                commentDtos.Add(new CommentDto() { State = state });
                return commentDtos;
            }
            state.Flag = true;
            state.Message = "comment Added Successfully";
            foreach (Comment comment in comments)
            {
                CommentDto commentDto = new CommentDto()
                {
                    State = state,
                    Id = comment.Id,
                    Content = comment.Content,
                    UserName = await _authService.GetUserName(comment.UserId),
                    Up = comment.Up,
                    Down = comment.Down,
                    Date = DateTime.Now,
                };
                commentDtos.Add(commentDto);
            }
            return commentDtos;
        }

        public async Task<CommentDto> EditComment(int commentId, string userId, EditDto model)
        {
            Comment? comment = await _context.Comments.SingleOrDefaultAsync(c => c.Id == commentId);
            StateDto state = new StateDto() { Flag = false, Message = "there is no comment with this id" };
            if (comment is null)
                return new CommentDto() { State = state };
            if (comment.UserId != userId)
            {
                state.Flag = false;
                state.Message = "Access Denied";
                return new CommentDto() { State = state };
            }
            state.Flag = true;
            state.Message = "Edited Successfullly";
            comment.Content = model.Content;
            await _context.SaveChangesAsync();
            return await GetCommentById(comment.Id, userId);

        }
        private async Task<CommentDto> GetCommentById(int id, string userId)
        {
            Comment? comment = await _context.Comments.SingleOrDefaultAsync(c => c.Id == id);
            StateDto state = new StateDto() { Flag = false, Message = "there is no comment with this id" };
            if (comment is null)
                return new CommentDto() { State = state };
            state.Flag = true;
            state.Message = "done";
            return new CommentDto()
            {
                State = state,
                Content = comment.Content,
                Date = comment.Date,
                Up = comment.Up
            ,
                Down = comment.Down,
                UserName = userId
            };
        }

        public async Task<StateDto> DeleteComment(int commentId, int postId, string userId)
        {
            Comment? comment = await _context.Comments.SingleOrDefaultAsync(c => c.Id == commentId);
            Post? post = await _context.Posts.SingleOrDefaultAsync(c => c.Id == postId);
            StateDto state = new StateDto() { Flag = false, Message = "there is no comment with this id" };
            if (comment is null)
                return state;
            if (comment.UserId != userId && post.UserId != userId)
            {
                state.Flag = false;
                state.Message = "Access Denied";
                return state;

            }
            state.Flag = true;
            state.Message = "Deleted Successfullly";
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return state;

        }


        public async Task<StateDto> AddComment(int postId, string userId, EditDto model)

        {
            Comment comment = new Comment();
            StateDto state = new StateDto() { Flag = false, Message = "Couidnt accept empty comment" };
            if (model.Content == "")
            {
                state.Flag = false;
                state.Message = "Couidnt accept empty comment";
                return state;
            }
            state.Flag = true;
            state.Message = "Added Successfully";
            comment.Content = model.Content;
            comment.UserId = userId;
            comment.PostId = postId;
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return state;
        }

    }
}
