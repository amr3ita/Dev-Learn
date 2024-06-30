using Code_Road.Dto.Account;
using Code_Road.Dto.Post;

namespace Code_Road.Services.PostService
{
    public interface IPostService
    {
        public Task<List<PostDto>> GetAllAsync();
        public Task<List<PostDto>> GetAllByUserIdAsync(string user_id);
        public Task<PostAndCommentsDto> GetByIdAsync(int post_id);
        public Task<PostDto> AddPostAsync(AddPostDto postModel);
        public Task<PostDto> UpdatePostAsync(int post_id, UpdatePostDto postModel);
        public Task<StateDto> DeletePostAsync(int post_id);

        Task<StateDto> IncreaseUpvoteAsync(int postId);
        Task<StateDto> IncreaseDownvoteAsync(int postId);
        Task<StateDto> DecreaseUpvoteAsync(int postId);
        Task<StateDto> DecreaseDownvoteAsync(int postId);

    }
}
