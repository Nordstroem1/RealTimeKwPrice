using Application.DTOs.User;
using Application.Users.Commands.CreateUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.UserControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CreateUserController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserController> _logger;
        public CreateUserController(IMediator mediator, ILogger<UserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        [HttpPost]
        [HttpPost("createUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _mediator.Send(new CreateUserCommand(user));

                if (result == null || !result.Succeeded)
                {
                    _logger.LogError("Failed to create user");
                    return BadRequest(new { result.FailLocation, result.Data, result.ErrorMessage, result.Succeeded });
                }

                return CreatedAtAction(nameof(CreateUser), new { id = result.Data.Id }, result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create user: {ex.Message}. Location: Controller");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
