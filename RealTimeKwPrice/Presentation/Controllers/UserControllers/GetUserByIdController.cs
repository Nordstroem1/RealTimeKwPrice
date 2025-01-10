using Application.Users.Queries.GetUserById;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace API.Controllers.UserControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GetUserByIdController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GetUserByIdController> _logger;

        public GetUserByIdController(IMediator mediator, ILogger<GetUserByIdController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var command = new GetUserByIdCommand(id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user retrieval");
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _mediator.Send(command);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("User with ID {UserId} not found", id);
                    return NotFound(new { ErrorMessage = result.ErrorMessage, Location = result.FailLocation });
                }

                _logger.LogInformation("User with ID {UserId} retrieved successfully", id);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user with ID {UserId}", id);
                return StatusCode(500, "An error occurred while retrieving the user");
            }
        }
    }
}