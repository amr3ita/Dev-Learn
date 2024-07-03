using Code_Road.Dto.Account;
using Code_Road.Dto.Account.Enum;
using Code_Road.Dto.Comment;
using Code_Road.Dto.Post;
using Code_Road.Helpers;
using Code_Road.Models;
using Code_Road.Services.EmailService;
using Code_Road.Services.UserService;
using Code_Road.Services.VotesService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Code_Road.Services.PostService.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _Jwt;
        private readonly IEmailService _emailService;
        private readonly UrlHelperFactoryService _urlHelperFactoryService;
        private readonly IWebHostEnvironment _environment;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPostService _postService;
        private readonly IVoteService _voteService;

        public AuthService(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> Jwt, IEmailService emailService, UrlHelperFactoryService urlHelperFactoryService, IUserService userService, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor, IPostService postService, IVoteService voteService)

        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _Jwt = Jwt.Value;
            _emailService = emailService;
            _urlHelperFactoryService = urlHelperFactoryService;
            _environment = environment;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _postService = postService;
            _voteService = voteService;
        }

        // Get All Users
        public async Task<List<UsersDto>> GetAllUsers()
        {
            var admin = await GetCurrentUserAsync();
            var usersDtoList = new List<UsersDto>();
            if (admin.IsAdmin == true)
            {
                var users = await _userManager.Users.ToListAsync();
                foreach (var user in users)
                {
                    var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                    usersDtoList.Add(new UsersDto
                    {
                        Name = $"{user.FirstName} {user.LastName}",
                        UserName = user.UserName,
                        Email = user.Email,
                        IsAdmin = isAdmin
                    });
                }
                return usersDtoList;
            }
            return null;
        }

        // Register
        public async Task<AuthDto> RegisterAsync(SignUpDto model, string requestScheme)
        {
            StateDto state = new StateDto();
            // If email is exist
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
            {
                state.Flag = false;
                state.Message = "This Email Is Exist!!";
                return new AuthDto() { Status = state };
            }
            // If userName is exit
            if (await _userManager.FindByNameAsync(model.Username) is not null)
            {
                state.Flag = false;
                state.Message = "This UserName Is Exist!!";
                return new AuthDto() { Status = state };
            }

            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Username,
                ActiceDay = DateTime.Now,
                OnlineDays = 0
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                string errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description}";
                }
                state.Flag = false;
                state.Message = errors;
                return new AuthDto { Status = state };
            }

            await _userManager.AddToRoleAsync(user, Roles.User.ToString());

            // Create Token
            var jwtSecurityToken = await CreateJwtToken(user);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = _urlHelperFactoryService.Action("VerifyEmail", "Auth", new { userId = user.Id, token }, requestScheme);
            var message = $"Please verify your email by clicking <a href='{confirmationLink}'>here</a>.";
            await _emailService.SendEmailAsync(model.Email, "Verify your email", message);

            state.Flag = true;
            state.Message = "Registration successful. Please check your email to verify your account.";
            await SetDefaultAvatarImage(user.Id);


            return new AuthDto
            {
                Status = state,
                UserName = user.UserName,
                Email = user.Email,
                LastCountinusActiveDays = 0,
                TokenExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { Roles.User.ToString() },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)
            };


        }
        // Verify Email
        public async Task<IdentityResult> VerifyEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "Invalid User ID" });

            return await _userManager.ConfirmEmailAsync(user, token);
        }

        // Generate Token
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim("uid",user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var JwtSecurityToken = new JwtSecurityToken(
                issuer: _Jwt.Issuer,
                audience: _Jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_Jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return JwtSecurityToken;
        }

        // Check User Login
        public async Task<AuthDto> LoginAsync(LoginDto model)
        {
            StateDto state = new StateDto();
            var authModel = new AuthDto();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                state.Flag = false;
                state.Message = "Email or Password is incorrect!";
                authModel.Status = state;
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            state.Flag = true;
            state.Message = "Login Successfully";

            authModel.Status = state;
            authModel.Email = user.Email;
            authModel.LastCountinusActiveDays = await _userService.ActiveDays(user.Id);
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.UserName = user.UserName;
            authModel.TokenExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = rolesList.ToList();
            return authModel;
        }

        // Add User To Role
        public async Task<StateDto> AddUserToRoleAsync(AddUserToRoleDto model)
        {
            var admin = await GetCurrentUserAsync();
            if (admin.IsAdmin == true)
            {
                var user = await _userManager.FindByEmailAsync(model.UserEmail);

                // if user not found
                if (user is null)
                    return new StateDto() { Flag = false, Message = "User Not Found" };
                // if role not found
                if (await _roleManager.RoleExistsAsync(model.Role) == false)
                    return new StateDto() { Flag = false, Message = $"{model.Role} Role Not Found" };

                // if user is assigned to this role
                if (await _userManager.IsInRoleAsync(user, model.Role) == true)
                    return new StateDto() { Flag = false, Message = "User is already assigned to this role" };

                await _userManager.AddToRoleAsync(user, model.Role);
                return new StateDto() { Flag = true, Message = "User Added To Role Successfully" };
            }
            return new StateDto { Flag = false, Message = "You Have No Permission to Do That!!" };
        }

        // get user name from id
        public async Task<string> GetUserName(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user is not null)
                return user.FirstName + " " + user.LastName;
            return string.Empty;
        }

        // Update Name
        public async Task<StateDto> UpdateName(string FirstName, string LastName)
        {
            StateDto state = new StateDto();
            try
            {
                var currentUser = await GetCurrentUserAsync();
                if (currentUser is not null)
                {
                    currentUser.userInfo.FirstName = FirstName;
                    currentUser.userInfo.LastName = LastName;

                    await _userManager.UpdateAsync(currentUser.userInfo);
                    await _context.SaveChangesAsync();

                    state.Flag = true;
                    state.Message = "Updated Successfully";
                }
                else
                {
                    state.Flag = false;
                    state.Message = "Something Error!!";
                }
            }
            catch (Exception ex)
            {
                state.Flag = false;
                state.Message = $"Error: {ex.Message}";
            }
            return state;
        }

        // Update User Name
        public async Task<StateDto> UpdateUserName(string userName)
        {
            StateDto state = new StateDto();
            if (await _userManager.FindByNameAsync(userName) is not null)
            {
                state.Flag = false;
                state.Message = "This UserName Is Exist!!";
                return state;
            }
            try
            {
                var currentUser = await GetCurrentUserAsync();
                if (currentUser is not null)
                {
                    if (await _context.Comments_Vote.AnyAsync(cv => cv.UserName == currentUser.userInfo.UserName))
                    {
                        List<CommentVote> cv = await _context.Comments_Vote.Where(cv => cv.UserName == currentUser.userInfo.UserName).ToListAsync();
                        foreach (var vote in cv)
                        {
                            vote.UserName = userName;
                        }
                        _context.Comments_Vote.UpdateRange(cv);
                    }
                    currentUser.userInfo.UserName = userName;

                    await _userManager.UpdateAsync(currentUser.userInfo);
                    await _context.SaveChangesAsync();

                    state.Flag = true;
                    state.Message = "Updated Successfully";
                }
                else
                {
                    state.Flag = false;
                    state.Message = "Something Error!!";
                }
            }
            catch (Exception ex)
            {
                state.Flag = false;
                state.Message = $"Error: {ex.Message}";
            }
            return state;
        }

        // Update Password
        public async Task<StateDto> UpdatePassword(UpdatePasswordDto model)
        {
            StateDto status = new StateDto();
            var user = await _userManager.FindByEmailAsync(model.Gmail);
            if (user is not null)
            {
                try
                {
                    bool isTrue = await _userManager.CheckPasswordAsync(user, model.OldPassword);
                    if (isTrue)
                    {
                        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                        if (result.Succeeded)
                        {
                            status.Flag = true;
                            status.Message = "Password Updated Successfully";
                            return status;
                        }
                    }
                }
                catch (Exception ex)
                {

                    status.Flag = false;
                    status.Message = $"Error: {ex.Message}";
                    return status;
                }

            }
            status.Flag = false;
            status.Message = "Old Password or Email Incorrect";
            return status;
        }

        // admin Delete User
        public async Task<StateDto> DeleteUser(string userEmail)
        {
            StateDto status = new StateDto();

            var currentUserDetails = await GetCurrentUserAsync();

            var currentUser = currentUserDetails.userInfo;
            if (currentUser is null)
            {
                status.Flag = false;
                status.Message = "Login first";
                return status;
            }
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                status.Flag = false;
                status.Message = "User Email Incorrect";
                return status;
            }

            bool isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            if (!isAdmin)
            {
                status.Flag = false;
                status.Message = "You don't have permission to delete user";
                return status;
            }

            try
            {
                // Delete related data
                await DeleteUserRelatedDataAsync(user.Id);

                // Delete user from role and delete user
                var result = await _userManager.RemoveFromRoleAsync(user, "User");
                if (result.Succeeded)
                {
                    result = await _userManager.DeleteAsync(user);
                    await _context.SaveChangesAsync();
                    if (result.Succeeded)
                    {
                        status.Flag = true;
                        status.Message = "User Deleted Successfully";
                        return status;
                    }
                }

                status.Flag = false;
                status.Message = string.Join("; ", result.Errors.Select(e => e.Description));
                return status;
            }
            catch (Exception ex)
            {
                status.Flag = false;
                status.Message = $"Error: {ex.Message}";
                return status;
            }
        }

        // user delete his account
        public async Task<StateDto> DeleteUserAccount()
        {
            StateDto status = new StateDto();

            var currentUserDetails = await GetCurrentUserAsync();

            var currentUser = currentUserDetails.userInfo;
            if (currentUser is null)
            {
                status.Flag = false;
                status.Message = "Login first";
                return status;
            }

            try
            {
                // Delete related data
                await DeleteUserRelatedDataAsync(currentUser.Id);

                // Delete user from role and delete user
                var result = await _userManager.RemoveFromRolesAsync(currentUser, new List<string> { "User", "Admin" });
                if (result.Succeeded)
                {
                    result = await _userManager.DeleteAsync(currentUser);
                    await _context.SaveChangesAsync();
                    if (result.Succeeded)
                    {
                        status.Flag = true;
                        status.Message = "User Deleted Successfully";
                        return status;
                    }
                }

                status.Flag = false;
                status.Message = string.Join("; ", result.Errors.Select(e => e.Description));
                return status;
            }
            catch (Exception ex)
            {
                status.Flag = false;
                status.Message = $"Error: {ex.Message}";
                return status;
            }
        }
        private async Task DeleteUserRelatedDataAsync(string userId)
        {
            // Delete comment Votes
            _context.Comments_Vote.RemoveRange(await _context.Comments_Vote.Where(cv => cv.UserId == userId).ToListAsync());
            // Delete comments
            _context.Comments.RemoveRange(await _context.Comments.Where(c => c.UserId == userId).ToListAsync());
            // Delete posts
            _context.Posts.RemoveRange(await _context.Posts.Where(p => p.UserId == userId).ToListAsync());
            // Delete finished lessons
            _context.FinishedLessons.RemoveRange(await _context.FinishedLessons.Where(fl => fl.UserId == userId).ToListAsync());
            // Delete images
            _context.Image.RemoveRange(await _context.Image.Where(i => i.UserId == userId).ToListAsync());

            // Unfollow users this user follows
            var followers = await _userService.GetAllFollowers(userId);
            foreach (var follower in followers.FollowersList)
            {
                await _userService.UnFollow(userId, follower.Id);
            }

            // Remove this user from other users' following lists
            var followings = await _userService.GetAllFollowing(userId);
            foreach (var follow in followings.FollowingList)
            {
                await _userService.UnFollow(follow.Id, userId);
            }

            // Save changes to database
            await _context.SaveChangesAsync();
        }

        // get current logged-in user
        public async Task<AllUserDataDto> GetCurrentUserAsync()
        {
            AllUserDataDto user = new AllUserDataDto();
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext.User == null)
            {
                return null;
            }
            string id = httpContext.User.FindFirstValue("uid") ?? string.Empty;
            user.userInfo = await _userManager.FindByIdAsync(id);
            user.IsAdmin = await _userManager.IsInRoleAsync(user.userInfo, "Admin");
            // get posts for this user
            user.posts = await _userService.GetAllByUserIdAsync(user.userInfo.Id);

            // get finished lessons for this user
            var finishedLessons = await _userService.GetFinishedLessonsForSpecificUser(user.userInfo.Id);
            user.finishedLessons = finishedLessons.FinishedLessons;

            // get the image of this user
            user.userImage = await _userService.GetUserImage(user.userInfo.Id);

            // get list user Votes
            var userPosts = await _voteService.UserPostVotes(user.userInfo.Id);
            var userComments = await _voteService.UserCommentVotes(user.userInfo.Id);

            user.UserVotes = new UserVotesDto
            {
                PostVotesId = new UserPostVotesDto { Status = new StateDto { Flag = true, Message = $"there is {userPosts.UpPosts.Count} up post and {userPosts.DownPosts.Count} down post" }, UpPosts = userPosts.UpPosts, DownPosts = userPosts.DownPosts },
                CommentVotesId = new UserCommentVotesDto { Status = new StateDto { Flag = true, Message = $"there is {userComments.UpComments.Count} up comment and {userComments.DownComments.Count} down comment" }, UpComments = userComments.UpComments, DownComments = userComments.DownComments }
            };


            if (user is null)
                return null;
            return user;
        }

        private async Task<StateDto> SetDefaultAvatarImage(string userId)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                string hosturl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
                Image image = new Image() { ImageUrl = hosturl + "/Upload/User/Avatar/Avatar.jpg", UserId = userId };
                await _context.AddAsync(image);
                await _context.SaveChangesAsync();
                return new StateDto() { Flag = true, Message = "Added Successfully" };
            }
            catch (Exception ex)
            {
                return new StateDto() { Flag = false, Message = ex.Message };
            }
        }

    }
}
