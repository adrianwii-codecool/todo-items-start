using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using TodoItems.Configurations.Options;
using TodoItems.Data;
using TodoItems.DTO;
using TodoItems.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TodoItems.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly JwtConfiguration _jwtConfiguration;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, IOptions<JwtConfiguration> jwtConfiguration)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _jwtConfiguration = jwtConfiguration.Value ?? throw new ArgumentNullException(nameof(jwtConfiguration));
        }

        private IActionResult ReportError(IdentityResult result)
        {

            IEnumerable<string> error = result.Errors.Select(e => e.Description);
            return BadRequest(new {Error = error});

        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerData)
        {
            User user = new User();
            user.UserName = registerData.UsernName;
            user.Email = registerData.Email;

            IdentityResult result =  await _userManager.CreateAsync(user, registerData.Password);

            if (!result.Succeeded)
            {
                return ReportError(result);
            }

            result = await _userManager.AddToRolesAsync(user, registerData.Roles);

            if(!result.Succeeded) {
                return ReportError(result);
            }

            return Accepted($"User '{user.UserName}' has been created");
        }

        // SESSION AUTHENTICATION

        [HttpPost("login-cookie")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginCookie([FromBody] LoginDTO? loginDTO)
        {
            if(loginDTO == null)
            {
                return BadRequest(new { Error = "Provide login data" });
            }

            User? user = await _userManager.FindByNameAsync(loginDTO.UserName);

            if(user == null)
            {
                return Unauthorized(loginDTO);
            }

            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);


            if (!result.Succeeded)
            {
                return Unauthorized(result);
            }

            // CREATE SESSION

            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName!));

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true,
                ExpiresUtc = DateTime.UtcNow.AddDays(1)
            });

            return Accepted();
        }

        [HttpPost("logout-cookie")]
        [AllowAnonymous]
        public async Task<IActionResult> LogoutCookie()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Accepted();
        }

        // TOKEN AUTHENTICATION JWT

        [HttpPost("login-jwt")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginJWT([FromBody] LoginDTO loginDTO)
        {
            if (loginDTO == null)
            {
                return BadRequest(new { Error = "Provide login data" });
            }

            User? user = await _userManager.FindByNameAsync(loginDTO.UserName);

            if (user == null)
            {
                return Unauthorized(loginDTO);
            }

            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);


            if (!result.Succeeded)
            {
                return Unauthorized(result);
            }

            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName!));

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var signInCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.SigningKey)), SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: _jwtConfiguration.Issuer,
                audience: _jwtConfiguration.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: signInCredentials
                ); 

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return Accepted(new { Token = token });
        }
    }
}
