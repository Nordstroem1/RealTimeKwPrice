using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using Application.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace API.Controllers.UserControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UpdateUserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UpdateUserController> _logger;

        public UpdateUserController(IMediator mediator, ILogger<UpdateUserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPut("{id}", Name = "UpdateUser")]
        [SwaggerOperation(Summary = "Update User", Description = "Updates an existing user by ID")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user update");
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                _logger.LogWarning("User ID mismatch for user update");
                return BadRequest("User ID mismatch");
            }

            var command = new UpdateUserCommand(id, user);
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
            {
                if (result.ErrorMessage == "User not found")
                {
                    _logger.LogWarning("User with ID {UserId} not found", id);
                    return NotFound(result.ErrorMessage);
                }
                _logger.LogError("Failed to update user with ID {UserId}: {ErrorMessage}", id, result.ErrorMessage);
                return BadRequest(result.ErrorMessage);
            }

            _logger.LogInformation("User with ID {UserId} updated successfully", id);
            return Ok(result.Data);
        }
    }
}