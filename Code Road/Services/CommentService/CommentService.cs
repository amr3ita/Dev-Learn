using Code_Road.Dto.Account;
using Code_Road.Dto.Comment;
using Code_Road.Dto.User;
using Code_Road.Models;
using Code_Road.Services.PostService;
using Code_Road.Services.PostService.AuthService;
using Code_Road.Services.UserService;
using Microsoft.EntityFrameworkCore;

namespace Code_Road.Services.CommentService
{
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IPostService _postService;

        public CommentService(AppDbContext context, IAuthService authService, IPostService postService, IUserService userService)
        {
            _context = context;
            _authService = authService;
            _postService = postService;
            _userService = userService;
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
            state.Flag = true;
            state.Message = "comment Added Successfully";
            var comments = await _context.Comments.Include(u => u.User).OrderByDescending(i => i.Up).Where(t => t.PostId == PostId).Select(comment => new CommentDto
            {
                State = state,
                Id = comment.Id,
                UserId = comment.UserId,
                Content = comment.Content,
                UserName = comment.User.FirstName + " " + comment.User.LastName,//await _authService.GetUserName(comment.UserId),
                UserImage = comment.User.Image.ImageUrl,// await _userService.GetUserImage(comment.UserId),
                Up = comment.Up,
                Down = comment.Down,
                Date = comment.Date
            }).ToListAsync();
            if (comments.Count <= 0)
            {
                state.Flag = false;
                state.Message = "there is no comments";
                commentDtos.Add(new CommentDto() { State = state });
                return commentDtos;
            }

            return comments;
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
        public async Task<CommentDto> GetCommentById(int id, string userId)
        {
            Comment? comment = await _context.Comments.Include(u => u.User).SingleOrDefaultAsync(c => c.Id == id);
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
                Up = comment.Up,
                UserImage = await _userService.GetUserImage(userId),
                Down = comment.Down,
                UserName = comment.User.UserName,
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
            if (_context.Comments_Vote.Any(cv => cv.CommentId == commentId))
            {
                var votes = _context.Comments_Vote.Where(cv => cv.CommentId == commentId).ToList();
                _context.Comments_Vote.RemoveRange(votes);
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return state;

        }
        public async Task<StateDto> AddComment(int postId, string content)

        {
            StateDto state = new StateDto() { Flag = false, Message = "Couldn't accept empty comment" };
            if (content == "")
                return state;
            state.Message = "Post Not Found";
            if (!(await _postService.GetByIdAsync(postId)).State.Flag)
                return state;
            var user = await _authService.GetCurrentUserAsync();
            state.Flag = true;
            state.Message = "Added Successfully";
            Comment comment = new Comment()
            {
                Content = content,
                UserId = user.userInfo.Id,
                PostId = postId,
                Up = 0,
                Down = 0,
                Date = DateTime.Now
            };

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return state;
        }
        public async Task<List<UsersReactDto>> GetUpVotes(int commentId)
        {
            Comment? comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
            List<UsersReactDto> users = new List<UsersReactDto>();
            StateDto state = new StateDto { Flag = false, Message = "there is no comment with this id" };
            if (comment == null)
            {
                users.Add(new UsersReactDto { State = state });
                return users;
            }
            var votes = _context.Comments_Vote
                .Where(cv => cv.CommentId == commentId && cv.Vote == 1)
                .Select(u => new { u.UserId, u.UserName, u.ImageUrl })
                .ToList();
            state.Message = "there is no Up Votes yet";
            if (votes.Count == 0)
            {
                users.Add(new UsersReactDto { State = state });
                return users;
            }
            else
            {
                state.Flag = true;
                state.Message = "added";
                foreach (var vote in votes)
                {
                    users.Add(new UsersReactDto { State = state, UserId = vote.UserId, UserName = vote.UserName, ImageUrl = vote.ImageUrl });
                }
                return users;
            }
        }
        public async Task<List<UsersReactDto>> GetDownVotes(int commentId)
        {
            Comment? comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
            List<UsersReactDto> users = new List<UsersReactDto>();
            StateDto state = new StateDto { Flag = false, Message = "there is no comment with this id" };
            if (comment == null)
            {
                users.Add(new UsersReactDto { State = state });
                return users;
            }
            var votes = _context.Comments_Vote
                .Where(cv => cv.CommentId == commentId && cv.Vote == 0)
                .Select(u => new { u.UserId, u.UserName, u.ImageUrl })
                .ToList();
            state.Message = "there is no Down Votes yet";
            if (votes.Count == 0)
            {
                users.Add(new UsersReactDto { State = state });
                return users;
            }
            else
            {
                state.Flag = true;
                state.Message = "added";
                foreach (var vote in votes)
                {
                    users.Add(new UsersReactDto { State = state, UserId = vote.UserId, UserName = vote.UserName, ImageUrl = vote.ImageUrl });
                }
                return users;
            }
        }

    }
}
