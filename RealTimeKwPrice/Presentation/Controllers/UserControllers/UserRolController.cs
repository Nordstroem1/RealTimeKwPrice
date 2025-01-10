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

            try
            {
                var command = new ChangeUserRoleCommand(changeUserRoleDTO);
                var operationResult = await _mediator.Send(command);

                if (!operationResult.Succeeded)
                {
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
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while processing the request.",
                    Exception = ex.Message,
                    StackTrace = ex.StackTrace 
                });
            }
        }
    }
}
