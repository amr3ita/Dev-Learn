using Code_Road.Dto.Account;
using Code_Road.Dto.User;

namespace Code_Road.Services.UserService
{
    public interface IUserService
    {
        Task<FollowersDto> GetAllFollowers(string id);
        Task<FollowingDto> GetAllFollowing(string id);
        Task<StateDto> Follow(string followerId, string followingId);
        Task<StateDto> UnFollow(string followerId, string followingId);
    }
}
