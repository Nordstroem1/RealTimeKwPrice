using Application.Commands;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Infrastructure.DependencyInjection;
using Infrastructure.Data;
using Infrastructure.Initializer;
using Microsoft.EntityFrameworkCore;
using Application.TokenHelper;
using Application.DataValidation.ExplicitWordList;

namespace Presentation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {


            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddHttpClient();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddInfrastructureLayer(builder.Configuration,
                builder.Configuration.GetConnectionString("DefaultConnection")!);
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    name: "LocalHostReactApp",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod();
                    }
                    );
            });


            // Register MediatR
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UpdateUserCommandHandler).Assembly));
            builder.Services.AddTransient<IGenericRepository<User>, GenericRepository<User>>();

            builder.Services.AddTransient<TokenHelper>();
            builder.Services.AddTransient<CheckForExplicitWord>(provider => new CheckForExplicitWord("path/to/explicitWords.json"));

            var app = builder.Build();
            app.UseCors("LocalHostReactApp");

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                await RoleInitializer.InitializeAsync(roleManager);
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}