using Application.Users.Commands.DeleteUser;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.UserControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeleteUserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DeleteUserController> _logger;
        private readonly ILoggerRepository _loggerToDatabase;

        public DeleteUserController(IMediator mediator, ILogger<DeleteUserController> logger, ILoggerRepository loggerToDatabase)
        {
            _mediator = mediator;
            _logger = logger;
            _loggerToDatabase = loggerToDatabase;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id, [FromBody] User user)
        {
            try
            {
                if (user == null)
                {
                    var errorMessage = "User object is required for deletion";
                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(DeleteUserController),
                        WhatWentWrong = errorMessage,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(DeleteUser)
                    });

                    _logger.LogWarning(errorMessage);
                    return BadRequest("User object is required.");
                }

                if (!ModelState.IsValid)
                {
                    var errorMessage = "Invalid model state for user deletion";
                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(DeleteUserController),
                        WhatWentWrong = errorMessage,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(DeleteUser)
                    });

                    _logger.LogWarning(errorMessage);
                    return BadRequest(ModelState);
                }

                var command = new DeleteUserCommand(id, user);
                var result = await _mediator.Send(command);

                if (!result.Succeeded)
                {
                    var errorMessage = $"Failed to delete user with ID {id}: {result.ErrorMessage}";
                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(DeleteUserController),
                        WhatWentWrong = errorMessage,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(DeleteUser)
                    });

                    _logger.LogError(errorMessage);
                    return BadRequest(new { ErrorMessage = result.ErrorMessage, Location = result.FailLocation });
                }

                _logger.LogInformation("User with ID {UserId} deleted successfully", id);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occurred while deleting user with ID {id}: {ex.Message}";
                await _loggerToDatabase.LogErrorAsync(new Logger
                {
                    Location = nameof(DeleteUserController),
                    WhatWentWrong = errorMessage,
                    TimeStamp = DateTime.UtcNow,
                    Function = nameof(DeleteUser)
                });

                _logger.LogError(ex, errorMessage);
                return StatusCode(500, "An error occurred while deleting the user");
            }
        }
    }
}