using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, OperationResult<User>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<CreateUserCommandHandler> _logger;
        private readonly ILoggerRepository _loggerToDatabse;

        public CreateUserCommandHandler(UserManager<User> userManager, ILogger<CreateUserCommandHandler> logger, ILoggerRepository loggerToDatabse)
        {
            _userManager = userManager;
            _logger = logger;
            _loggerToDatabse = loggerToDatabse;
        }

        public async Task<OperationResult<User>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var createdUser = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = request.UserDto.UserName,
                    Email = request.UserDto.Email,
                    PhoneNumber = request.UserDto.PhoneNumber,
                    CreatedAt = request.UserDto.CreatedAt,
                    Role = request.UserDto.Role,
                    Location = request.UserDto.Location,
                    PriceList = request.UserDto.PriceList
                };

                var userCreationResult = await _userManager.CreateAsync(createdUser, request.UserDto.Password);

                if (!userCreationResult.Succeeded)
                {
                    var errors = string.Join(", ", userCreationResult.Errors.Select(e => e.Description));
                    _logger.LogError($"Error when creating a user: {errors}");

                    await _loggerToDatabse.LogErrorAsync(new Logger
                    {
                        Location = nameof(CreateUserCommandHandler),
                        WhatWentWrong = errors,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(Handle)
                    });

                    return OperationResult<User>.Fail($"Failed to create user: {errors}", "Application");
                }

                var roleResult = await _userManager.AddToRoleAsync(createdUser, createdUser.Role.ToString());

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    _logger.LogError($"Error when assigning role to user: {errors}");

                    await _loggerToDatabse.LogErrorAsync(new Logger
                    {
                        Location = nameof(CreateUserCommandHandler),
                        WhatWentWrong = errors,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(Handle)
                    });

                    return OperationResult<User>.Fail($"Failed to create user: {errors}", "Application");
                }

                _logger.LogInformation("User added successfully.");

                return OperationResult<User>.Success(createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError($"(Unexpected error). Error when creating a user: {ex.Message}");

                await _loggerToDatabse.LogErrorAsync(new Logger
                {
                    Location = nameof(CreateUserCommandHandler),
                    WhatWentWrong = ex.Message,
                    TimeStamp = DateTime.UtcNow,
                    Function = nameof(Handle)
                });

                throw new Exception("Unexpected error." + ex.Message);
            }
        }
    }
}
