using Code_Road.Dto.Account;

namespace Code_Road.Services.PostService.AuthService
{
    public interface IAuthService
    {
        Task<List<UsersDto>> GetAllUsers();
        Task<AuthDto> RegisterAsync(SignUpDto model);
        Task<AuthDto> LoginAsync(LoginDto model);
        Task<StateDto> AddUserToRoleAsync(AddUserToRoleDto model);
        Task<string> GetUserName(string Id);
        Task<StateDto> UpdatePassword(UpdatePasswordDto model);
        Task<StateDto> DeleteUser(DeleteUserDto model);
    }
}
