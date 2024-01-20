using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoItems.Data;
using TodoItems.DTO;
using TodoItems.Models;

namespace TodoItems.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly TodoContext _context;
        private readonly UserManager<User> _userManager;

        public UserController(TodoContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<TodoItem>> Register([FromBody] RegisterDTO registerData)
        {
            User user = new User();
            user.UserName = registerData.UsernName;
            user.Email = registerData.Email;

            IdentityResult result =  await _userManager.CreateAsync(user, registerData.Password);

            if (!result.Succeeded)
            {
                // TODO throw exception
            }

            result = await _userManager.AddToRolesAsync(user, registerData.Roles);

            if(!result.Succeeded) { 
                // TODO throw exception
            }

            return Accepted($"User '{user.UserName}' has been created");
        }
    }
}
