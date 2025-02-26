using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Entities;

public interface IUserService
{
    string HashPassword(User user, string password);

    Task<User?> GetUserById(int ID);

    Task<User?> RegisterAsync(UserDto request);

    Task<AuthResponseDto?> LoginAsync(AuthLoginDto request);

    Task<AuthResponseDto?> RefreshTokenAsync(AuthRefreshTokenRequestDto request);

    Task<User?> Logout(int ID);
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

    public async Task<User?> GetUserById(int ID)
    {
        return await _dbContext.Users.FindAsync(ID);
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

    public async Task<AuthResponseDto?> LoginAsync(AuthLoginDto request)
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

        var response = await CreateTokenResponseAsync(user);

        return response;
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, user.Username),
            };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
            audience: _configuration.GetValue<string>("AppSettings:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    private string CreateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    private async Task<string> GenerateRefreshTokenAsync(User user)
    {
        var refreshToken = CreateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _dbContext.SaveChangesAsync();
        return refreshToken;
    }

    private async Task<User?> ValidateRefreshTokenAsync(int ID, string refreshToken)
    {
        var user = await _dbContext.Users.FindAsync(ID);

        if (
            user == null
            || user.RefreshToken != refreshToken
            || user.RefreshTokenExpiry <= DateTime.UtcNow)
        {
            return null;
        }

        return user;
    }

    private async Task<AuthResponseDto> CreateTokenResponseAsync(User user) {
        var tokens = new AuthResponseDto
        {
            AccessToken = CreateToken(user),
            RefreshToken = await GenerateRefreshTokenAsync(user),
        };

       return tokens;
    }
    
    public async Task<AuthResponseDto?> RefreshTokenAsync(AuthRefreshTokenRequestDto request) {
        var user = await ValidateRefreshTokenAsync(request.ID, request.RefreshToken);

        if (user == null) {
            return null;
        }

        return await CreateTokenResponseAsync(user);
    }

    public async Task<User?> Logout (int ID) {
        var user = await GetUserById(ID);

        if (user == null) {
            return null;
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;

        await _dbContext.SaveChangesAsync();

        return user;
    }
}