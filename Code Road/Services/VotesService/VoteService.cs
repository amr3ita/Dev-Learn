using Code_Road.Dto.Account;
using Code_Road.Dto.Comment;
using Code_Road.Dto.Post;
using Code_Road.Models;
using Code_Road.Services.PostService;
using Code_Road.Services.UserService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Code_Road.Services.VotesService
{
    public class VoteService : IVoteService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IPostService _postService;
        public VoteService(AppDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, IUserService userService, IPostService postService)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _postService = postService;
            _userService = userService;
        }
        // posts
        public async Task<StateDto> PostVote(int postId, int vote)
        {
            var user = await GetCurrentUserAsync();
            PostVote postVote = await _context.Posts_Vote.FirstOrDefaultAsync(c => c.PostId == postId && c.UserId == user.userInfo.Id);
            Post? post = await _context.Posts.FirstOrDefaultAsync(c => c.Id == postId);
            StateDto state = new StateDto() { Flag = false, Message = "there is no posts may be it deleted" };
            if (post is null)
                return state;

            if (vote == 1 || vote == 0)
            {
                if (postVote is not null)
                {
                    if (postVote.Vote == 1 && vote == 1)
                    {
                        _context.Posts_Vote.Remove(postVote);
                        post.Up--;
                        _context.Posts.Update(post);
                        await _context.SaveChangesAsync();
                        return new StateDto { Flag = true, Message = "Up vote is deleted" };
                    }
                    else if (postVote.Vote == 0 && vote == 0)
                    {
                        _context.Posts_Vote.Remove(postVote);
                        post.Down--;
                        _context.Posts.Update(post);
                        await _context.SaveChangesAsync();
                        return new StateDto { Flag = true, Message = "Down vote is deleted" };
                    }
                    else if (postVote.Vote == 1 && vote == 0)
                    {
                        post.Up--;
                        post.Down++;
                        postVote.Vote = vote;
                        _context.Posts.Update(post);
                        _context.Posts_Vote.Update(postVote);
                        await _context.SaveChangesAsync();
                        return new StateDto { Flag = true, Message = "Your Vote Updated" };
                    }
                    else if (postVote.Vote == 0 && vote == 1)
                    {
                        post.Up++;
                        post.Down--;
                        postVote.Vote = vote;
                        _context.Posts.Update(post);
                        _context.Posts_Vote.Update(postVote);
                        await _context.SaveChangesAsync();
                        return new StateDto { Flag = true, Message = "Your Vote Updated" };
                    }
                    else
                    {
                        return new StateDto { Flag = true, Message = "something went wrong" };
                    }
                }
                else
                {
                    postVote = new PostVote { PostId = postId, Vote = vote, UserId = user.userInfo.Id, UserName = user.userInfo.UserName, ImageUrl = user.userImage };
                    await _context.Posts_Vote.AddAsync(postVote);
                    if (vote == 1)
                    {
                        post.Up++;
                    }
                    else
                    {
                        post.Down++;
                    }
                    _context.Posts.Update(post);
                    await _context.SaveChangesAsync();
                    state.Flag = true;
                    state.Message = "Voted Successfully";
                    return state;
                }
            }
            else
            {
                state.Flag = false;
                state.Message = "InValidInput {up = 1 , down = 0}";
                return state;
            }
        }

        public async Task<UserPostVotesDto> UserPostVotes(string userId)
        {
            UserPostVotesDto userPostVotes = new UserPostVotesDto();
            var posts = await _context.Posts_Vote.Where(p => p.UserId == userId).ToListAsync();
            if (posts.Count > 0)
            {
                userPostVotes.Status = new StateDto { Flag = true, Message = $"user have vote {posts.Count} post" };
                userPostVotes.UpPosts = posts.Where(p => p.Vote == 1).Select(p => p.PostId).ToList();
                userPostVotes.DownPosts = posts.Where(p => p.Vote == 0).Select(p => p.PostId).ToList();
            }
            else
            {
                userPostVotes.Status = new StateDto { Flag = false, Message = "user doesn't vote any post" };
                userPostVotes.UpPosts = new List<int>();
                userPostVotes.DownPosts = new List<int>();
            }
            return userPostVotes;
        }


        // comments
        public async Task<StateDto> CommentVote(int commentId, int vote)
        {
            var user = await GetCurrentUserAsync();
            CommentVote commentvote = await _context.Comments_Vote.FirstOrDefaultAsync(c => c.CommentId == commentId && c.UserId == user.userInfo.Id);
            Comment? comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
            StateDto state = new StateDto() { Flag = false, Message = "there is no Comment may be it deleted" };
            if (comment is null)
                return state;

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
                        _context.Comments.Update(comment);
                        _context.Comments_Vote.Update(commentvote);
                        await _context.SaveChangesAsync();
                        return new StateDto { Flag = true, Message = "Your Vote Updated" };
                    }
                    else if (commentvote.Vote == 0 && vote == 1)
                    {
                        comment.Up++;
                        comment.Down--;
                        commentvote.Vote = vote;
                        _context.Comments.Update(comment);
                        _context.Comments_Vote.Update(commentvote);
                        await _context.SaveChangesAsync();
                        return new StateDto { Flag = true, Message = "Your Vote Updated" };
                    }
                    else
                    {
                        return new StateDto { Flag = true, Message = "something went wrong" };
                    }
                }
                else
                {
                    commentvote = new CommentVote { CommentId = commentId, Vote = vote, UserId = user.userInfo.Id, UserName = user.userInfo.UserName, ImageUrl = user.userImage };
                    _context.Comments_Vote.Add(commentvote);
                    if (vote == 1)
                    {
                        comment.Up++;
                    }
                    else
                    {
                        comment.Down++;
                    }
                    _context.Comments.Update(comment);
                    await _context.SaveChangesAsync();
                    state.Flag = true;
                    state.Message = "Voted Successfully";
                    return state;
                }
            }
            else
            {
                state.Flag = false;
                state.Message = "InValidInput {up = 1 , down = 0}";
                return state;
            }
        }

        public async Task<UserCommentVotesDto> UserCommentVotes(string userId)
        {
            UserCommentVotesDto userPostVotes = new UserCommentVotesDto();
            var comments = await _context.Comments_Vote.Where(c => c.UserId == userId).ToListAsync();
            if (comments.Count > 0)
            {
                userPostVotes.Status = new StateDto { Flag = true, Message = $"user Up {comments.Count} comment" };
                userPostVotes.UpComments = comments.Where(c => c.Vote == 1).Select(c => c.CommentId).ToList();
                userPostVotes.DownComments = comments.Where(c => c.Vote == 0).Select(c => c.CommentId).ToList();
            }
            else
            {
                userPostVotes.Status = new StateDto { Flag = false, Message = "user doesn't vote any comment" };
                userPostVotes.UpComments = new List<int>();
                userPostVotes.DownComments = new List<int>();
            }
            return userPostVotes;
        }

        private async Task<AllUserDataDto> GetCurrentUserAsync()
        {
            AllUserDataDto user = new AllUserDataDto();
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext.User == null)
            {
                return null;
            }
            string id = httpContext.User.FindFirstValue("uid") ?? string.Empty;
            user.userInfo = await _userManager.FindByIdAsync(id);

            // get the image of this user
            user.userImage = await _userService.GetUserImage(user.userInfo.Id);

            if (user is null)
                return null;
            return user;
        }
    }
}
