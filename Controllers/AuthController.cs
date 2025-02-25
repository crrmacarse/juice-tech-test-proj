using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Entities;

// TODO: logout, role-based
namespace WebApplication1
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService) {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var user = await _userService.RegisterAsync(request);

            if (user == null) {
                return BadRequest("Invalid Request");
            }

            return Ok("Registered Successfully");

            // return CreatedAtAction(nameof(_userService.GetUserById), new { id = user.ID }, user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(AuthLoginDto request) {
            var token = await _userService.LoginAsync(request);

            if (token == null) {
                return BadRequest("Invalid username or password");
            }

            return Ok(token);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult> Profile() {
            return Ok("Authorized!");
        }
    }
}
