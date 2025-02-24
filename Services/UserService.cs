using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

public interface IUserService
{
    string HashPassword(User user, string password);

    Task<User?> GetUserById(int id);
}

public class UserService: IUserService
{
    private readonly AppDbContext _dbContext;

    public UserService(AppDbContext dbContext)
    {

        _dbContext = dbContext;
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
}