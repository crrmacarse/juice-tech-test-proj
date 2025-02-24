using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserService _userService;

        public UserController(AppDbContext dbContext, IUserService userService)
        {

            _dbContext = dbContext;
            _userService = userService;
        }

        [HttpGet]
        public async Task<List<User>> GetUsers()
        {
            var users = await _dbContext.Users.ToListAsync();

            return users;
        }

        [HttpGet("{id}")]
        public async Task<User?> GetUserById(int id)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.ID == id);
        }

        // [HttpPost]
        // public async Task<ActionResult> CreateUser([FromBody] User request) {
        //     if (string.IsNullOrEmpty(request.FullName) ||
        //         string.IsNullOrEmpty(request.Username) ||
        //         string.IsNullOrEmpty(request.Password)
        //     ) {
        //         return BadRequest("Missing fields");
        //     }


        // }
    }
}
