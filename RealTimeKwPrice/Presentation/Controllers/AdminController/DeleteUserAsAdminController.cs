using Application.Users.Commands.DeleteUserAsAdmin;
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

        public DeleteUserAsAdminController(IMediator mediator, ILogger<DeleteUserAsAdminController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserAsAdmin(Guid id)
        {
            try
            {
                var command = new DeleteUserAsAdminCommand(id);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for admin user deletion");
                    return BadRequest(ModelState);
                }

                var result = await _mediator.Send(command);

                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to delete user with ID {UserId}: {ErrorMessage}", id, result.ErrorMessage);
                    return BadRequest(new { ErrorMessage = result.ErrorMessage, Location = result.FailLocation });
                }

                _logger.LogInformation("User with ID {UserId} deleted successfully by admin", id);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting user with ID {UserId}", id);
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }
    }
}