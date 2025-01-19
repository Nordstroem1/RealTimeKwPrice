using Application.DTO.User;
using Application.Users.Queries.LogInUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.UserControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginUserController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<LoginUserController> _logger;

        public LoginUserController(IMediator mediator, ILogger<LoginUserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserQuery loginQuery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _mediator.Send(loginQuery);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Login failed: {ErrorMessage}, Location: {FailLocation}",
                        result.ErrorMessage, result.FailLocation);

                    return Unauthorized(new
                    {
                        Message = result.ErrorMessage,
                        Location = result.FailLocation
                    });
                }

                _logger.LogInformation("User {UserName} logged in successfully", loginQuery.UserName);

                var (user, token) = result.Data;

                return Ok(new
                {
                    Message = "Login successful",
                    User = new
                    {
                        user.Id,
                        user.UserName,
                        user.Email,
                        user.Role
                    },
                    Token = token
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login for user {UserName}", loginQuery.UserName);
                return StatusCode(500, new { Message = "Internal server error", Exception = ex.Message });
            }
        }
    }
}
