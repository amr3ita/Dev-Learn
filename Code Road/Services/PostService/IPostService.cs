using Code_Road.Dto.Account;
using Code_Road.Dto.Post;

namespace Code_Road.Services.PostService
{
    public interface IPostService
    {
        public Task<List<PostAndCommentsDto>> GetAllAsync();
        public Task<List<PostDto>> GetAllByUserIdAsync(string user_id);
        public Task<PostAndCommentsDto> GetByIdAsync(int post_id);
        public Task<PostDto> AddPostAsync(AddPostDto postModel);
        public Task<PostDto> UpdatePostAsync(int post_id, UpdatePostDto postModel);
        public Task<StateDto> DeletePostAsync(int post_id);

        public Task<StateDto> IncreaseUpvoteAsync(int postId);
        public Task<StateDto> IncreaseDownvoteAsync(int postId);
        public Task<StateDto> DecreaseUpvoteAsync(int postId);
        public Task<StateDto> DecreaseDownvoteAsync(int postId);
    }
}
