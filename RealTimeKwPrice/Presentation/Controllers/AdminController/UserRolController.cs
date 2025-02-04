using Application.DTO.Role;
using Application.Users.Commands.ChangeUserRole;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserRolController> _logger; 
        private readonly ILoggerRepository _loggerToDatabase; 

        public UserRolController(IMediator mediator, ILogger<UserRolController> logger, ILoggerRepository loggerToDatabase)
        {
            _mediator = mediator;
            _logger = logger;
            _loggerToDatabase = loggerToDatabase;
        }

        [HttpPost("change-role")]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeUserRoleDTO changeUserRoleDTO)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = "Invalid model state for changing user role.";

                await _loggerToDatabase.LogErrorAsync(new Logger
                {
                    Location = nameof(UserRolController),
                    WhatWentWrong = errorMessage,
                    TimeStamp = DateTime.UtcNow,
                    Function = nameof(ChangeUserRole)
                });

                _logger.LogWarning(errorMessage);

                return BadRequest(ModelState);
            }

            try
            {
                var command = new ChangeUserRoleCommand(changeUserRoleDTO);
                var operationResult = await _mediator.Send(command);

                if (!operationResult.Succeeded)
                {
                    var errorMessage = "Failed to change user role.";

                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(UserRolController),
                        WhatWentWrong = errorMessage,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(ChangeUserRole)
                    });

                    _logger.LogError(errorMessage);

                    return BadRequest(new
                    {
                        operationResult.ErrorMessage,
                        operationResult.FailLocation
                    });
                }

                return Ok(operationResult.Data);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An unexpected error occurred while processing the request: {ex.Message}";

                await _loggerToDatabase.LogErrorAsync(new Logger
                {
                    Location = nameof(UserRolController),
                    WhatWentWrong = errorMessage,
                    TimeStamp = DateTime.UtcNow,
                    Function = nameof(ChangeUserRole)
                });

                _logger.LogError(errorMessage);

                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while processing the request.",
                    Exception = ex.Message,
                    ex.StackTrace
                });
            }
        }
    }
}
