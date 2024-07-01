using Code_Road.Dto.Account;
using Code_Road.Dto.Post;
using Code_Road.Dto.User;

namespace Code_Road.Services.PostService
{
    public interface IPostService
    {
        public Task<List<PostAndCommentsDto>> GetAllAsync();
        public Task<List<PostDto>> GetAllByUserIdAsync(string user_id);
        public Task<PostAndCommentsDto> GetByIdAsync(int post_id);
        public Task<PostDto> AddPostAsync(AllUserDataDto currentUser, AddPostDto postModel);
        public Task<PostDto> UpdatePostAsync(AllUserDataDto currentUser, int post_id, UpdatePostDto postModel);
        public Task<StateDto> DeletePostAsync(AllUserDataDto currentUser, int post_id);
        public Task<List<UsersReactDto>> GetUpVotes(int postId);
        public Task<List<UsersReactDto>> GetDownVotes(int postId);
    }
}
