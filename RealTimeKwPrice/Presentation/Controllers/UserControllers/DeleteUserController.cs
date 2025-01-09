using Application.Users.Commands.DeleteUser;
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

        public DeleteUserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id, [FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User object is required.");
            }

            var command = new DeleteUserCommand(id, user);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(command);

            if (!result.Succeeded)
            {
                return BadRequest(new { ErrorMessage = result.ErrorMessage, Location = result.FailLocation });
            }

            return Ok(result.Data);
        }
    }
}