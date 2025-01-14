using Application.DTO.Role;
using Application.Users.Commands.ChangeUserRole;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.SuperAdminController
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChangeAdminRoleAsSuperAdminController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ChangeAdminRoleAsSuperAdminController> _logger;
        public ChangeAdminRoleAsSuperAdminController(IMediator mediator, ILogger<ChangeAdminRoleAsSuperAdminController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Authorize(Policy = "SuperAdmin")]
        [HttpPost("changeAdminRole")]
        public async Task<IActionResult> ChangeAdminRole([FromBody] ChangeUserRoleDTO changeAdminRoleDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _mediator.Send( new ChangeUserRoleCommand(changeAdminRoleDTO));

                if(!result.Succeeded)
                {
                    return BadRequest(new { result.Data, result.ErrorMessage, result.FailLocation});
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
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
