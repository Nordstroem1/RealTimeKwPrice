using Application.Users.Queries.GetAllUsers;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.UserControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GetAllUsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GetAllUsersController> _logger;

        public GetAllUsersController(IMediator mediator, ILogger<GetAllUsersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user retrieval");
                return BadRequest(ModelState);
            }

            try
            {
                var command = new GetAllUsersCommand();
                var result = await _mediator.Send(command);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to retrieve users: {ErrorMessage}", result.ErrorMessage);
                    return NotFound(new { ErrorMessage = result.ErrorMessage, Location = result.FailLocation });
                }

                _logger.LogInformation("Users retrieved successfully");
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users");
                return StatusCode(500, "An error occurred while retrieving users");
            }
        }
    }
}