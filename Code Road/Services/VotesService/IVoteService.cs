using Code_Road.Dto.Account;
using Code_Road.Dto.Comment;
using Code_Road.Dto.Post;

namespace Code_Road.Services.VotesService
{
    public interface IVoteService
    {
        public Task<StateDto> PostVote(int postId, int vote);
        public Task<UserPostVotesDto> UserPostVotes(string userId);
        public Task<StateDto> CommentVote(int commentId, int vote);
        public Task<UserCommentVotesDto> UserCommentVotes(string userId);
    }
}
