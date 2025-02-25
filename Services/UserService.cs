using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Entities;

public interface IUserService
{
    string HashPassword(User user, string password);

    Task<User?> GetUserById(int id);

    Task<User?> RegisterAsync(UserDto request);

    Task<string?> LoginAsync(AuthLoginDto request);
}

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;
            private readonly IConfiguration _configuration;

    public UserService(AppDbContext dbContext, IConfiguration configuration)
    {

        _dbContext = dbContext;
        _configuration = configuration;

    }


    public string HashPassword(User user, string password)
    {
        var hashedPassword = new PasswordHasher<User>()
        .HashPassword(user, password);

        return hashedPassword;
    }

    public async Task<User?> GetUserById(int id)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(x => x.ID == id);
    }

    public async Task<User?> RegisterAsync(UserDto request)
    {
        if (await _dbContext.Users.AnyAsync(u => u.Username.ToLower() == request.Username.ToLower()))
        {
            return null;
        }

        var user = new User();
        var hashedPassword = HashPassword(user, request.Password);

        user.FullName = request.FullName;
        user.Username = request.Username;
        user.Password = hashedPassword;

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<string?> LoginAsync(AuthLoginDto request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == request.Username.ToLower());

        if (user == null)
        {
            return null;
        }

        if (new PasswordHasher<User>().VerifyHashedPassword(user, user.Password, request.Password)
        == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return CreateToken(user);
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString())
            };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
            audience: _configuration.GetValue<string>("AppSettings:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}