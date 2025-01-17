using Application.Users.Queries.GetAllUsers;
using Domain.Interfaces;
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
        private readonly ILoggerRepository _loggerToDatabase;

        public GetAllUsersController(IMediator mediator, ILogger<GetAllUsersController> logger, ILoggerRepository loggerToDatabase)
        {
            _mediator = mediator;
            _logger = logger;
            _loggerToDatabase = loggerToDatabase;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errorMessage = "Invalid model state for user retrieval";
                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(GetAllUsersController),
                        WhatWentWrong = errorMessage,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(GetAllUsers)
                    });

                    _logger.LogError(errorMessage);
                    return BadRequest(ModelState);
                }

                var command = new GetAllUsersCommand();
                var result = await _mediator.Send(command);

                if (!result.Succeeded)
                {
                    var errorMessage = $"Failed to retrieve users: {result.ErrorMessage}";
                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(GetAllUsersController),
                        WhatWentWrong = errorMessage,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(GetAllUsers)
                    });

                    _logger.LogError(errorMessage);
                    return NotFound(new { ErrorMessage = result.ErrorMessage, Location = result.FailLocation });
                }

                _logger.LogInformation("Users retrieved successfully");
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occurred while retrieving users: {ex.Message}";
                await _loggerToDatabase.LogErrorAsync(new Logger
                {
                    Location = nameof(GetAllUsersController),
                    WhatWentWrong = errorMessage,
                    TimeStamp = DateTime.UtcNow,
                    Function = nameof(GetAllUsers)
                });

                _logger.LogError(ex, errorMessage);
                return StatusCode(500, "An error occurred while retrieving users");
            }
        }
    }
}