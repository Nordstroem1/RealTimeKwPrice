using Application.DTO;
using Application.Users.Commands.ChangeUserRole;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers.UserControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserRolController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Policy = "Admin")]
        [HttpPost("change-role")]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeUserRoleDTO changeUserRoleDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new ChangeUserRoleCommand(changeUserRoleDTO);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
