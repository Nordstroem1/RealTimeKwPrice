using MediatR;
using Domain.Models;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.GetAllUsers
{
    public class GetAllUsersCommandHandler : IRequestHandler<GetAllUsersCommand, OperationResult<List<User>>>
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<GetAllUsersCommandHandler> _logger;
        private readonly ILoggerRepository _loggerToDatabse;


        public GetAllUsersCommandHandler(IGenericRepository<User> userRepository, UserManager<User> userManager, ILogger<GetAllUsersCommandHandler> logger, ILoggerRepository loggerToDatabse)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _logger = logger;
            _loggerToDatabse = loggerToDatabse;
        }

        public async Task<OperationResult<List<User>>> Handle(GetAllUsersCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                if (users == null || !users.Any())
                {
                    _logger.LogWarning("No users found");

                    await _loggerToDatabse.LogErrorAsync(new Logger
                    {
                        Location = nameof(GetAllUsersCommandHandler),
                        WhatWentWrong = "No users found",
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(Handle)
                    });

                    return OperationResult<List<User>>.Fail("No users found", nameof(GetAllUsersCommandHandler));
                }

                _logger.LogInformation("Users retrieved successfully");
                return OperationResult<List<User>>.Success(users.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users");

                await _loggerToDatabse.LogErrorAsync(new Logger
                {
                    Location = nameof(GetAllUsersCommandHandler),
                    WhatWentWrong = ex.Message,
                    TimeStamp = DateTime.UtcNow,
                    Function = nameof(Handle)
                });

                return OperationResult<List<User>>.Fail("An error occurred while retrieving users", nameof(GetAllUsersCommandHandler));
            }
        }
    }
}