using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options => {
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnection"));
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(Options => {
        Options.Title = "Test Project";
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
