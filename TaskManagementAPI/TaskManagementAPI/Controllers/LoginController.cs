using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Model;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public LoginController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel user)
        {
            var result = await _signInManager.PasswordSignInAsync(user.UserName, user.Password, false, false);
            if (!result.Succeeded)
                return Unauthorized(new { Message = "Invalid credentials" });
            var userDetail = await _userManager.FindByNameAsync(user.UserName);

            return Ok(new { Message = "Login successful", Token = TokenService.GenerateToken(userDetail, _userManager) });
        }

    }
}
