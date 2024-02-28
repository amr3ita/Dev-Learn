using Code_Road.Dto.Account;

namespace Code_Road.Services.PostService.AuthService
{
    public interface IAuthService
    {
        Task<AuthDto> RegisterAsync(SignUpDto model);
        Task<AuthDto> LoginAsync(LoginDto model);
        Task<StateDto> AddUserToRoleAsync(AddUserToRoleDto model);
    }
}
