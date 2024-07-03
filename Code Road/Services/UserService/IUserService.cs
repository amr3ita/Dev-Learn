using Code_Road.Dto.Account;
using Code_Road.Dto.Post;
using Code_Road.Dto.User;

namespace Code_Road.Services.UserService
{
    public interface IUserService
    {
        Task<FollowersDto> GetAllFollowers(string id);
        Task<FollowingDto> GetAllFollowing(string id);
        Task<StateDto> Follow(string followerId, string followingId);
        Task<StateDto> UnFollow(string followerId, string followingId);
        Task<FinishedLessonsDto> GetFinishedLessonsForSpecificUser(string userId);
        Task<StateDto> FinishLesson(string userId, int lessonId, int degree);
        Task<StateDto> UpdateUserImage(string userId, IFormFile image);
        Task<StateDto> DeleteUserImage(string userId);
        public Task<string> GetUserImage(string userId);
        public Task<int> ActiveDays(string userId);
        Task<UserProfileDto> GetUserById(string id);
        Task<List<PostDto>> GetAllByUserIdAsync(string user_id);

    }
}
