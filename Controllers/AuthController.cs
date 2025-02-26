using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Entities;

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
        public async Task<ActionResult<AuthResponseDto>> Login(AuthLoginDto request) {
            var result = await _userService.LoginAsync(request);

            if (result == null) {
                return BadRequest("Invalid username or password");
            }

            return Ok(result);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult> Profile() {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await _userService.GetUserById(int.Parse(userId));

            if (response == null) {
                return Unauthorized("User not found");
            }

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponseDto>> CreateRefreshToken(AuthRefreshTokenRequestDto request) {
            var response = await _userService.RefreshTokenAsync(request);

            if (response is null || response.AccessToken is null || response.RefreshToken is null) {
                return Unauthorized("Invalid Token");
            }

            return Ok(response);
        }

        [HttpGet("logout")]
        public async Task<ActionResult> Logout() {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) {
                return BadRequest("Bad request");
            }

            await _userService.Logout(int.Parse(userId));

            return Ok("Logout successfully");
        }
    }
}
