using GoogleOAuth2ScopeSample.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tenon.AspNetCore.Authentication.Google.Extensions.Extensions;

namespace GoogleOAuth2ScopeSample;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.
        builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            options.UseSqlite(builder.Configuration["ConnectionStrings:DefaultConnection"]));
        builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAuthentication()
            .AddGoogleScope(builder.Configuration.GetSection("Authentication:Google"), opt =>
            {
                opt.Events.OnCreatingTicket = context =>
                {
                    Console.WriteLine($"[AccessToken]:{context.AccessToken}");
                    Console.WriteLine($"[RefreshToken]:{context.RefreshToken}");
                    return Task.CompletedTask;
                };
            });
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors(x => x
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true) // allow any origin
            .AllowCredentials());
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}