using Application.TokenHelper;
using Application.Users.Queries.GetUserById;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.LogInUser
{
    public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, OperationResult<(User, string)>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<LoginUserQueryHandler> _logger;
        private readonly TokenHelper.TokenHelper _tokenHelper;

        public LoginUserQueryHandler(
            UserManager<User> userManager,
            ILogger<LoginUserQueryHandler> logger,
            TokenHelper.TokenHelper tokenHelper)
        {
            _userManager = userManager;
            _logger = logger;
            _tokenHelper = tokenHelper;
        }

        public async Task<OperationResult<(User, string)>> Handle(LoginUserQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(request.UserName);
                if (user == null)
                {
                    _logger.LogWarning("User with username {UserName} not found", request.UserName);
                    return OperationResult<(User, string)>.Fail("Invalid username or password", nameof(LoginUserQueryHandler));
                }

                var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
                if (!passwordValid)
                {
                    _logger.LogWarning("Invalid password for user {UserName}", request.UserName);
                    return OperationResult<(User, string)>.Fail("Invalid username or password", nameof(LoginUserQueryHandler));
                }

                var token = _tokenHelper.GenerateToken(user);

                _logger.LogInformation("User {UserName} logged in successfully", request.UserName);

                return OperationResult<(User, string)>.Success((user, token));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in user {UserName}", request.UserName);
                return OperationResult<(User, string)>.Fail("An error occurred during login", nameof(LoginUserQueryHandler));
            }
        }
    }
}
