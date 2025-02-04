using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using Application.Commands;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers.UserControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UpdateUserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UpdateUserController> _logger;
        private readonly ILoggerRepository _loggerToDatabase;

        public UpdateUserController(IMediator mediator, ILogger<UpdateUserController> logger, ILoggerRepository loggerToDatabase)
        {
            _mediator = mediator;
            _logger = logger;
            _loggerToDatabase = loggerToDatabase;
        }
        [HttpPut("{id}", Name = "UpdateUser")]
        [SwaggerOperation(Summary = "Update User", Description = "Updates an existing user by ID")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errorMessage = "Invalid model state for user update";

                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(UpdateUserController),
                        WhatWentWrong = errorMessage,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(UpdateUser)
                    });

                    _logger.LogError(errorMessage);
                    return BadRequest(ModelState);
                }

                if (id != user.Id)
                {
                    var errorMessage = "User ID mismatch for user update";

                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(UpdateUserController),
                        WhatWentWrong = errorMessage,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(UpdateUser)
                    });

                    _logger.LogError(errorMessage);
                    return BadRequest("User ID mismatch");
                }

                var command = new UpdateUserCommand(id, user);
                var result = await _mediator.Send(command);

                if (!result.Succeeded)
                {
                    if (result.ErrorMessage == "User not found")
                    {
                        var errorMessage = $"User with ID {id} not found";

                        await _loggerToDatabase.LogErrorAsync(new Logger
                        {
                            Location = nameof(UpdateUserController),
                            WhatWentWrong = errorMessage,
                            TimeStamp = DateTime.UtcNow,
                            Function = nameof(UpdateUser)
                        });

                        _logger.LogError(errorMessage);
                        return NotFound(result.ErrorMessage);
                    }

                    var errorMessageForUpdate = $"Failed to update user with ID {id}: {result.ErrorMessage}";

                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(UpdateUserController),
                        WhatWentWrong = errorMessageForUpdate,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(UpdateUser)
                    });

                    _logger.LogError(errorMessageForUpdate);
                    return BadRequest(result.ErrorMessage);
                }

                _logger.LogInformation("User with ID {UserId} updated successfully", id);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An unexpected error occurred while updating user with ID {id}: {ex.Message}";

                await _loggerToDatabase.LogErrorAsync(new Logger
                {
                    Location = nameof(UpdateUserController),
                    WhatWentWrong = errorMessage,
                    TimeStamp = DateTime.UtcNow,
                    Function = nameof(UpdateUser)
                });

                _logger.LogError(errorMessage);
                return StatusCode(500, "An unexpected error occurred");
            }
        }
    }
}