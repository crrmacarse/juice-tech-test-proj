using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{

    public DbSet<User> Users { get; set; }
}


