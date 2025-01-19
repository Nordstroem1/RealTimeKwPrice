using Application.TokenHelper;
using Application.Users.Queries.GetUserById;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.LogInUser
{
    public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, OperationResult<User>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<LoginUserQueryHandler> _logger;

        public LoginUserQueryHandler(
            UserManager<User> userManager,
            ILogger<LoginUserQueryHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<OperationResult<User>> Handle(LoginUserQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(request.UserName);
                if (user == null)
                {
                    _logger.LogWarning("User with username {UserName} not found", request.UserName);
                    return OperationResult<User>.Fail("Invalid username or password", nameof(LoginUserQueryHandler));
                }

                var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
                if (!passwordValid)
                {
                    _logger.LogWarning("Invalid password for user {UserName}", request.UserName);
                    return OperationResult<User>.Fail("Invalid username or password", nameof(LoginUserQueryHandler));
                }

                _logger.LogInformation("User {UserName} logged in successfully", request.UserName);
                return OperationResult<User>.Success(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in user {UserName}", request.UserName);
                return OperationResult<User>.Fail("An error occurred during login", nameof(LoginUserQueryHandler));
            }
        }
    }
}
