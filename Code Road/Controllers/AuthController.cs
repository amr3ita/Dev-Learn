using Code_Road.Dto.Account;
using Code_Road.Services.PostService.AuthService;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            if (ModelState.IsValid)
            {
                var users = await _authService.GetAllUsers();
                if (users is null)
                    return Unauthorized("You Have Permission to Do That!!");
                return Ok(users);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] SignUpDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            AuthDto user = await _authService.RegisterAsync(model, Request.Scheme);

            if (!user.Status.Flag)
                return Ok(user.Status.Message);

            return Ok(user);
        }

        [HttpGet("verifyemail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            var result = await _authService.VerifyEmail(userId, token);
            if (result.Succeeded)
            {
                return Ok("Email verified successfully");
            }
            else
            {
                return BadRequest("Email verification failed");
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            AuthDto user = await _authService.LoginAsync(model);
            if (!user.Status.Flag)
                return Ok(user.Status.Message);

            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddUserToRole")]
        public async Task<IActionResult> AddUserToRoleAsync(AddUserToRoleDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            StateDto state = await _authService.AddUserToRoleAsync(model);
            if (!state.Flag)
                return Ok(state.Message);

            return Ok(state.Message);
        }

        [Authorize]
        [HttpPut("UpdateName")]
        public async Task<IActionResult> UpdateName(string FirstName, string LastName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            StateDto status = await _authService.UpdateName(FirstName, LastName);
            if (!status.Flag)
                return Ok(status.Message);
            return Ok(status.Message);

        }

        [Authorize]
        [HttpPut("UpdateUSerName")]
        public async Task<IActionResult> UpdateUserName(string userName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            StateDto status = await _authService.UpdateUserName(userName);
            if (!status.Flag)
                return Ok(status.Message);
            return Ok(status.Message);

        }

        [Authorize]
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
                    return Ok(status.Message);
                }
                return Ok("Old Password and New Password Should be Different");
            }
            return BadRequest(ModelState);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("AdminDeleteUser")]
        public async Task<IActionResult> DeleteUserAsync(string userEmail)
        {

            if (ModelState.IsValid)
            {
                StateDto status = await _authService.DeleteUser(userEmail);
                if (status.Flag)
                    return Ok(new { Message = status.Message });
                return Ok(status.Message);
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUserAccountAsync()
        {

            if (ModelState.IsValid)
            {
                StateDto status = await _authService.DeleteUserAccount();
                if (status.Flag)
                    return Ok(new { Message = status.Message });
                return Ok(status.Message);
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpGet("GetCurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var user = await _authService.GetCurrentUserAsync();
                return Ok(user);

            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
