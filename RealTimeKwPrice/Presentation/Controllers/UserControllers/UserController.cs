using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using Application.Commands;

namespace API.Controllers.UserControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut("{id}", Name = "UpdateUser")]
        [SwaggerOperation(Summary = "Update User", Description = "Updates an existing user by ID")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest("User ID mismatch");
            }

            var command = new UpdateUserCommand(id, user);
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
            {
                if (result.ErrorMessage == "User not found")
                {
                    return NotFound(result.ErrorMessage);
                }
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
    }
}