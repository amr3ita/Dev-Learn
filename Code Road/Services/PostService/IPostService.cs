using Code_Road.Dto.Post;
using Code_Road.Dto.Account;
using Code_Road.Models;

namespace Code_Road.Services.PostService
{
    public interface IPostService
    {
        public Task<List<PostDto>> GetAllAsync();
        public Task<List<PostDto>> GetAllByUserIdAsync(string user_id);
        public Task<PostDto> GetByIdAsync(int post_id);
        public Task<PostDto> AddPostAsync(AddPostDto postModel);
        public Task<PostDto> UpdatePostAsync(int post_id, UpdatePostDto postModel);
        public Task<StateDto> DeletePostAsync(int post_id, string user_id);
        public Task<StateDto> VoteAsync(int postId, string userId, bool isVote);
        public Task<List<PostVoteDto>> GetAllVotesAsync(int postId);

    }
}
