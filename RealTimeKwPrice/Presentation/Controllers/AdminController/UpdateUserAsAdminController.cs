using Application.Commands;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Domain.Interfaces;

namespace API.Controllers.AdminControllers
{
    [ApiController]
    [Route("api/admin/[controller]")]
    public class UpdateUserAsAdminController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UpdateUserAsAdminController> _logger;
        private readonly ILoggerRepository _loggerToDatabase;

        public UpdateUserAsAdminController(IMediator mediator, ILogger<UpdateUserAsAdminController> logger, ILoggerRepository loggerToDatabase)
        {
            _mediator = mediator;
            _logger = logger;
            _loggerToDatabase = loggerToDatabase;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UpdateUserAsAdmin(Guid id, [FromBody] User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errorMessage = "Invalid model state for admin user update";

                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(UpdateUserAsAdminController),
                        WhatWentWrong = errorMessage,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(UpdateUserAsAdmin)
                    });

                    _logger.LogError(errorMessage);
                    return BadRequest(ModelState);
                }

                if (id != user.Id)
                {
                    var errorMessage = "User ID mismatch for admin user update";

                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(UpdateUserAsAdminController),
                        WhatWentWrong = errorMessage,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(UpdateUserAsAdmin)
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
                            Location = nameof(UpdateUserAsAdminController),
                            WhatWentWrong = errorMessage,
                            TimeStamp = DateTime.UtcNow,
                            Function = nameof(UpdateUserAsAdmin)
                        });

                        _logger.LogError(errorMessage);
                        return NotFound(result.ErrorMessage);
                    }

                    var errorMessageForUpdate = $"Failed to update user with ID {id}: {result.ErrorMessage}";

                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(UpdateUserAsAdminController),
                        WhatWentWrong = errorMessageForUpdate,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(UpdateUserAsAdmin)
                    });

                    _logger.LogError(errorMessageForUpdate);
                    return BadRequest(result.ErrorMessage);
                }

                _logger.LogInformation("User with ID {UserId} updated successfully by admin", id);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An unexpected error occurred while updating user with ID {id}: {ex.Message}";

                await _loggerToDatabase.LogErrorAsync(new Logger
                {
                    Location = nameof(UpdateUserAsAdminController),
                    WhatWentWrong = errorMessage,
                    TimeStamp = DateTime.UtcNow,
                    Function = nameof(UpdateUserAsAdmin)
                });

                _logger.LogError(errorMessage);
                return StatusCode(500, "An unexpected error occurred");
            }
        }
    }
}