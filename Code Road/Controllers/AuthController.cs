using Code_Road.Dto.Account;
using Code_Road.Services.PostService.AuthService;
using Microsoft.AspNetCore.Mvc;

namespace Code_Road.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // [Authorize(Roles = "Admin")]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _authService.GetAllUsers();

            return Ok(users);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] SignUpDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            AuthDto user = await _authService.RegisterAsync(model);
            if (!user.Status.Flag)
                return BadRequest(user.Status.Message);

            return Ok(user);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            AuthDto user = await _authService.LoginAsync(model);
            if (!user.Status.Flag)
                return BadRequest(user.Status.Message);

            return Ok(user);
        }

        [HttpPost("AddUserToRole")]
        public async Task<IActionResult> AddUserToRoleAsync(AddUserToRoleDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            StateDto state = await _authService.AddUserToRoleAsync(model);
            if (!state.Flag)
                return BadRequest(state.Message);

            return Ok(state.Message);
        }

        [HttpPut("UpdatePassword")]
        public async Task<IActionResult> UpdatePasswordAsync(UpdatePasswordDto model)
        {
            if (ModelState.IsValid)
            {
                if (!model.OldPassword.Equals(model.NewPassword))
                {
                    StateDto status = await _authService.UpdatePassword(model);
                    if (status.Flag)
                        return Ok(status.Message);
                    return BadRequest(status.Message);
                }
                return BadRequest("Old Password and New Password Should be Different");
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUserAsync(DeleteUserDto model)
        {
            if (ModelState.IsValid)
            {
                StateDto status = await _authService.DeleteUser(model);
                if (status.Flag)
                    return Ok(status.Message);
                return BadRequest(status.Message);
            }
            return BadRequest(ModelState);
        }

    }
}
