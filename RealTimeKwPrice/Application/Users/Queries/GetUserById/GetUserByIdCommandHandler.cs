using MediatR;
using Domain.Models;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Users.Queries.GetUserById
{
    public class GetUserByIdCommandHandler : IRequestHandler<GetUserByIdCommand, OperationResult<User>>
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<GetUserByIdCommandHandler> _logger;
        private readonly ILoggerRepository _loggerToDatabse;

        public GetUserByIdCommandHandler(IGenericRepository<User> userRepository, UserManager<User> userManager, ILogger<GetUserByIdCommandHandler> logger, ILoggerRepository loggerToDatabse)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _logger = logger;
            _loggerToDatabse = loggerToDatabse;
        }

        public async Task<OperationResult<User>> Handle(GetUserByIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.Id.ToString());
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", request.Id);

                    await _loggerToDatabse.LogErrorAsync(new Logger
                    {
                        Location = nameof(GetUserByIdCommandHandler),
                        WhatWentWrong = $"User with ID {request.Id} not found",
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(Handle)
                    });

                    return OperationResult<User>.Fail("User not found", nameof(GetUserByIdCommandHandler));
                }

                _logger.LogInformation("User with ID {UserId} retrieved successfully", request.Id);
                return OperationResult<User>.Success(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user with ID {UserId}", request.Id);

                await _loggerToDatabse.LogErrorAsync(new Logger
                {
                    Location = nameof(GetUserByIdCommandHandler),
                    WhatWentWrong = ex.Message,
                    TimeStamp = DateTime.UtcNow,
                    Function = nameof(Handle)
                });

                return OperationResult<User>.Fail("An error occurred while retrieving the user", nameof(GetUserByIdCommandHandler));
            }
        }
    }
}