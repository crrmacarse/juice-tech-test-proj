using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Entities;

namespace WebApplication1
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new();

        [HttpPost("register")]
        public ActionResult<User> Register(UserDto request)
        {
            var hashedPassword = new PasswordHasher<User>()
            .HashPassword(user, request.Password);

            user.Username = request.Username;
            user.Password = hashedPassword;

            return Ok(user);
        }

        [HttpPost("login")]
        public ActionResult<string> Login(UserDto request) {
            if (user.Username != request.Username) {
                return BadRequest("User not found");
            }

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.Password, request.Password)
            == PasswordVerificationResult.Failed) {
                return BadRequest("Wrong Password");
            }

            string token = "success";

            return Ok(token);
        }
    }
}
