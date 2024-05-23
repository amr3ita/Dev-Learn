using Code_Road.Dto.Account;
using Code_Road.Dto.User;
using Code_Road.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Code_Road.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _user;

        public UserService(AppDbContext context, UserManager<ApplicationUser> user)
        {
            _context = context;
            _user = user;
        }
        public async Task<FollowersDto> GetAllFollowers(string id)
        {
            ApplicationUser? user = await _user.FindByIdAsync(id);
            StateDto state = new StateDto() { Flag = false, Message = "invalid Id" };
            FollowersDto fd = new FollowersDto();
            if (user is null)
            {
                fd.Count = 0;
                fd.State = state;
                return fd;

            }
            state.Flag = false;
            state.Message = "there is now followers";

            var followers = await _context.Follow.Where(f => f.FollowingId == id).ToListAsync();
            if (followers is null)
            {
                fd.State = state;
                fd.Count = 0;
                return fd;
            }
            state.Flag = true;
            state.Message = "done";
            fd.State = state;
            foreach (var f in followers)
            {
                var folwee = await _user.FindByIdAsync(f.FollowerId);
                fd.FollowersList.Add(new UserDetailsDto { Id = f.FollowerId, UserName = f.Follower.UserName });

            }
            fd.Count = fd.FollowersList.Count;
            return fd;
        }
        public async Task<FollowingDto> GetAllFollowing(string id)
        {
            ApplicationUser? user = await _user.FindByIdAsync(id);
            StateDto state = new StateDto() { Flag = false, Message = "invalid Id" };
            FollowingDto fd = new FollowingDto();
            if (user is null)
            {
                fd.Count = 0;
                fd.State = state;
                return fd;

            }
            state.Flag = false;
            state.Message = "there is now followings";

            var followers = await _context.Follow.Where(f => f.FollowerId == id).ToListAsync();
            if (followers is null)
            {
                fd.State = state;
                fd.Count = 0;
                return fd;
            }
            state.Flag = true;
            state.Message = "done";
            fd.State = state;
            foreach (var f in followers)
            {
                var folwee = await _user.FindByIdAsync(f.FollowingId);
                fd.FollowingList.Add(new UserDetailsDto { Id = f.FollowerId, UserName = folwee.UserName });

            }
            fd.Count = fd.FollowingList.Count;
            return fd;
        }


        public async Task<StateDto> Follow(string followerId, string followingId)
        {
            ApplicationUser? follower = await _user.FindByIdAsync(followerId);
            ApplicationUser? following = await _user.FindByIdAsync(followingId);
            StateDto state = new StateDto { Flag = false, Message = "Invalid ID" };
            if (follower is null || following is null)
                return state;
            state.Flag = true;
            state.Message = "Following";
            Follow follow = new Follow() { FollowingId = followingId, FollowerId = followerId };
            if (!_context.Follow.Any(f => f.FollowingId == followingId && f.FollowerId == followerId))
            {
                _context.Follow.Add(follow);

                await _context.SaveChangesAsync();
                return state;
            }
            else
            {
                state.Flag = true;
                state.Message = "You Already Follow this Account";
                return state;
            }


        }
        public async Task<StateDto> UnFollow(string followerId, string followingId)
        {
            ApplicationUser? follower = await _user.FindByIdAsync(followerId);
            ApplicationUser? following = await _user.FindByIdAsync(followingId);

            if (follower is null || following is null)
                return new StateDto { Flag = false, Message = "Invalid ID" };
            Follow follow = new Follow() { FollowingId = followingId, FollowerId = followerId };
            if (_context.Follow.Any(f => f.FollowingId == followingId && f.FollowerId == followerId))
            {
                _context.Follow.Remove(follow);

                await _context.SaveChangesAsync();
                return new StateDto { Flag = true, Message = "Following removed" };
            }
            else
            {

                return new StateDto { Flag = true, Message = "You Already un Follow this Account" };
            }


        }


    }
}
