using Application.Users.Commands.DeleteUser;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace API.Controllers.AdminControllers
{
    [ApiController]
    [Route("api/admin/[controller]")]
    public class DeleteUserAsAdminController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DeleteUserAsAdminController> _logger;
        private readonly ILoggerRepository _loggerToDatabase;

        public DeleteUserAsAdminController(IMediator mediator, ILogger<DeleteUserAsAdminController> logger, ILoggerRepository loggerToDatabase)
        {
            _mediator = mediator;
            _logger = logger;
            _loggerToDatabase = loggerToDatabase;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteUserAsAdmin(Guid id)
        {
            try
            {
                var command = new DeleteUserCommand(id, null);

                if (!ModelState.IsValid)
                {
                    var errorMessage = "Invalid model state for admin user deletion";
                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(DeleteUserAsAdminController),
                        WhatWentWrong = errorMessage,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(DeleteUserAsAdmin)
                    });

                    _logger.LogWarning(errorMessage);
                    return BadRequest(ModelState);
                }

                var result = await _mediator.Send(command);

                if (!result.Succeeded)
                {
                    var errorMessage = $"Failed to delete user with ID {id}: {result.ErrorMessage}";
                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(DeleteUserAsAdminController),
                        WhatWentWrong = errorMessage,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(DeleteUserAsAdmin)
                    });

                    _logger.LogError(errorMessage);
                    return BadRequest(new { ErrorMessage = result.ErrorMessage, Location = result.FailLocation });
                }

                _logger.LogInformation("User with ID {UserId} deleted successfully by admin", id);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An unexpected error occurred while deleting user with ID {id}: {ex.Message}";
                await _loggerToDatabase.LogErrorAsync(new Logger
                {
                    Location = nameof(DeleteUserAsAdminController),
                    WhatWentWrong = errorMessage,
                    TimeStamp = DateTime.UtcNow,
                    Function = nameof(DeleteUserAsAdmin)
                });

                _logger.LogError(ex, errorMessage);
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }
    }
}