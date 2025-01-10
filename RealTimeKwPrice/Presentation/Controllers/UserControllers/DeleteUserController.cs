using Application.Users.Commands.DeleteUser;
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
    public class DeleteUserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DeleteUserController> _logger;

        public DeleteUserController(IMediator mediator, ILogger<DeleteUserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id, [FromBody] User user)
        {
            if (user == null)
            {
                _logger.LogWarning("User object is required for deletion");
                return BadRequest("User object is required.");
            }

            var command = new DeleteUserCommand(id, user);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user deletion");
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(command);

            if (!result.Succeeded)
            {
                _logger.LogError("Failed to delete user with ID {UserId}: {ErrorMessage}", id, result.ErrorMessage);
                return BadRequest(new { ErrorMessage = result.ErrorMessage, Location = result.FailLocation });
            }

            _logger.LogInformation("User with ID {UserId} deleted successfully", id);
            return Ok(result.Data);
        }
    }
}