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
                    UserImage = await _userService.GetUserImage(comment.UserId),
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

        public async Task<StateDto> Vote(int commentId, int vote)
        {
            var user = await _authService.GetCurrentUserAsync();
            CommentVote commentvote = await _context.Comments_Vote.FirstOrDefaultAsync(c => c.CommentId == commentId && c.UserId == user.userInfo.Id);
            Comment? comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
            StateDto state = new StateDto() { Flag = false, Message = "there is no Comment may be it deleted" };
            if (comment is null)
                return state;

            state.Message = "InValidInput {up = 1 , down = 0}";
            if (vote == 1 || vote == 0)
            {
                if (commentvote is not null)
                {
                    if (commentvote.Vote == 1 && vote == 1)
                    {
                        _context.Comments_Vote.Remove(commentvote);
                        comment.Up--;
                        _context.Comments.Update(comment);
                        await _context.SaveChangesAsync();
                        return new StateDto { Flag = true, Message = "Up vote is deleted" };
                    }
                    else if (commentvote.Vote == 0 && vote == 0)
                    {
                        _context.Comments_Vote.Remove(commentvote);
                        comment.Down--;
                        _context.Comments.Update(comment);
                        await _context.SaveChangesAsync();
                        return new StateDto { Flag = true, Message = "Down vote is deleted" };
                    }
                    else if (commentvote.Vote == 1 && vote == 0)
                    {
                        comment.Up--;
                        comment.Down++;
                        commentvote.Vote = vote;
                        _context.Comments_Vote.Update(commentvote);
                    }
                    else if (commentvote.Vote == 0 && vote == 1)
                    {
                        comment.Up++;
                        comment.Down--;
                        commentvote.Vote = vote;
                        _context.Comments_Vote.Update(commentvote);
                    }
                }
                else
                {
                    commentvote.Vote = vote;
                    commentvote.CommentId = commentId;
                    commentvote.UserId = user.userInfo.Id;
                    commentvote.UserName = user.userInfo.UserName;
                    commentvote.ImageUrl = user.userImage;
                    _context.Comments_Vote.Add(commentvote);
                    if (vote == 1)
                    {
                        comment.Up++;
                    }
                    else
                    {
                        comment.Down++;
                    }
                }
                _context.Comments.Update(comment);
                await _context.SaveChangesAsync();
                state.Flag = true;
                state.Message = "Voted Successfully";
                return state;
            }
            return state;
        }


    }
}
