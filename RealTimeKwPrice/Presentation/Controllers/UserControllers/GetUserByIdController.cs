using Application.Users.Queries.GetUserById;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ILoggerRepository _loggerToDatabase;

        public GetUserByIdController(IMediator mediator, ILogger<GetUserByIdController> logger, ILoggerRepository loggerToDatabase)
        {
            _mediator = mediator;
            _logger = logger;
            _loggerToDatabase = loggerToDatabase;
        }

        [Authorize(Policy = "User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var command = new GetUserByIdCommand(id);

                if (!ModelState.IsValid)
                {
                    var errorMessage = "Invalid model state for user retrieval";

                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(GetUserByIdController),
                        WhatWentWrong = errorMessage,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(GetUserById)
                    });

                    _logger.LogError(errorMessage);
                    return BadRequest(ModelState);
                }

                var result = await _mediator.Send(command);

                if (!result.Succeeded)
                {
                    var errorMessage = $"User with ID {id} not found";

                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(GetUserByIdController),
                        WhatWentWrong = errorMessage,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(GetUserById)
                    });

                    _logger.LogError(errorMessage);
                    return NotFound(new { ErrorMessage = result.ErrorMessage, Location = result.FailLocation });
                }

                _logger.LogInformation("User with ID {UserId} retrieved successfully", id);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occurred while retrieving user with ID {id}: {ex.Message}";

                await _loggerToDatabase.LogErrorAsync(new Logger
                {
                    Location = nameof(GetUserByIdController),
                    WhatWentWrong = errorMessage,
                    TimeStamp = DateTime.UtcNow,
                    Function = nameof(GetUserById)
                });

                _logger.LogError(ex, errorMessage);
                return StatusCode(500, "An error occurred while retrieving the user");
            }
        }
    }
}