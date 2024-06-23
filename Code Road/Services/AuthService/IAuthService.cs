using Code_Road.Dto.Account;
using Microsoft.AspNetCore.Identity;

namespace Code_Road.Services.PostService.AuthService
{
    public interface IAuthService
    {
        Task<List<UsersDto>> GetAllUsers();
        Task<AuthDto> RegisterAsync(SignUpDto model, string requestScheme);
        Task<IdentityResult> VerifyEmail(string userId, string token);
        Task<AuthDto> LoginAsync(LoginDto model);
        Task<StateDto> AddUserToRoleAsync(AddUserToRoleDto model);
        Task<string> GetUserName(string Id);
        Task<StateDto> UpdatePassword(UpdatePasswordDto model);
        Task<StateDto> DeleteUser(DeleteUserDto model);
    }
}
