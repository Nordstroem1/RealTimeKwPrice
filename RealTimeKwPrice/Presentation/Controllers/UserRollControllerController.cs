
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRollControllerController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public UserRollControllerController(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("ChangeRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeRole(Guid userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                var failResult = OperationResult<string>.Fail("User not found.", "UserRollController.ChangeRole");
                return NotFound(failResult);
            }

            if (user.UserName == User.Identity.Name)
            {
                var failResult = OperationResult<string>.Fail("You cannot change your own role.", "UserRollController.ChangeRole");
                return BadRequest(failResult);
            }

            if (!await _roleManager.RoleExistsAsync(newRole))
            {
                var failResult = OperationResult<string>.Fail("Role does not exist.", "UserRollController.ChangeRole");
                return BadRequest(failResult);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                var failResult = OperationResult<string>.Fail("Failed to remove existing roles.", "UserRollController.ChangeRole");
                return StatusCode(500, failResult);
            }

            var addResult = await _userManager.AddToRoleAsync(user, newRole);
            if (!addResult.Succeeded)
            {
                var failResult = OperationResult<string>.Fail("Failed to assign the new role.", "UserRollController.ChangeRole");
                return StatusCode(500, failResult);
            }

            var successResult = OperationResult<string>.Success($"User's role successfully changed to {newRole}.");
            return Ok(successResult);
        }
    }
}
