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
using Microsoft.OpenApi.Models;

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
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by a space and then your token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            builder.Services.AddInfrastructureLayer(builder.Configuration,
                builder.Configuration.GetConnectionString("DefaultConnection")!);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    name: "REALTIMEKWPRICEFE",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:3000")
                               .AllowAnyOrigin()
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UpdateUserCommandHandler).Assembly));
            builder.Services.AddTransient<IGenericRepository<User>, GenericRepository<User>>();

            builder.Services.AddTransient<TokenHelper>();
            builder.Services.AddTransient<CheckForExplicitWord>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<CheckForExplicitWord>>();
                return new CheckForExplicitWord("path/to/explicitWords.json", logger);
            });

            var app = builder.Build();
            app.UseCors("REALTIMEKWPRICEFE");

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
