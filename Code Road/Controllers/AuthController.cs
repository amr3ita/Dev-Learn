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
    }
}
