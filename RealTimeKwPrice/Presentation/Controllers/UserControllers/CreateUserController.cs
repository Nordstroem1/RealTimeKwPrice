using Application.DTO.User;
using Application.Users.Commands.CreateUser;
using Domain.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.UserControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CreateUserController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CreateUserController> _logger; 
        private readonly ILoggerRepository _loggerToDatabase; 

        public CreateUserController(IMediator mediator, ILogger<CreateUserController> logger, ILoggerRepository loggerToDatabase)
        {
            _mediator = mediator;
            _logger = logger;
            _loggerToDatabase = loggerToDatabase;
        }

        [HttpPost]
        [HttpPost("createUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errorMessage = "Invalid model state for user creation";

                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(CreateUserController),
                        WhatWentWrong = errorMessage,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(CreateUser)
                    });

                    _logger.LogWarning(errorMessage);

                    return BadRequest(ModelState);
                }

                var result = await _mediator.Send(new CreateUserCommand(user));

                if (result == null || !result.Succeeded)
                {
                    var errorMessage = "Failed to create user";

                    await _loggerToDatabase.LogErrorAsync(new Logger
                    {
                        Location = nameof(CreateUserController),
                        WhatWentWrong = errorMessage,
                        TimeStamp = DateTime.UtcNow,
                        Function = nameof(CreateUser)
                    });

                    _logger.LogError(errorMessage);

                    return BadRequest(new { result.FailLocation, result.Data, result.ErrorMessage, result.Succeeded });
                }

                _logger.LogInformation("User with ID {UserId} created successfully", result.Data.Id);

                return CreatedAtAction(nameof(CreateUser), new { id = result.Data.Id }, result.Data);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Failed to create user: {ex.Message}. Location: Controller";

                await _loggerToDatabase.LogErrorAsync(new Logger
                {
                    Location = nameof(CreateUserController),
                    WhatWentWrong = errorMessage,
                    TimeStamp = DateTime.UtcNow,
                    Function = nameof(CreateUser)
                });

                _logger.LogError(errorMessage);

                return StatusCode(500, "Internal server error");
            }
        }
    }
}
