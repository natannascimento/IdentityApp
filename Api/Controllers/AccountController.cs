using Api.DTO.Account;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JWTService _jwtService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        
        public AccountController(JWTService jwtService,
            SignInManager<User> signManager,
            UserManager<User> userManager)
        {
            _jwtService = jwtService;
            _signInManager = signManager;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if(user == null) return Unauthorized("username ou senha invalido");

            if (user.EmailConfirmed == false) return Unauthorized("Por favor confirme seu email.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized("username ou senha invalido");

            return CreateApplicationUserDto(user);

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if(await CheckEmailExitsAsync(model.Email))
            {
                return BadRequest($"Uma conta existente está usando {model.Email}, endereço de email. Por favor tetne com outro endreço de email ");
            }

            var userToAdd = new User
            {
                FirsName = model.FirstName.ToLower(),
                LastName = model.LastName.ToLower(),
                UserName = model.Email.ToLower(),
                Email = model.Email.ToLower(),
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(userToAdd, model.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok("Sua conta foi criada, você pode fazer o login ");
        }

        #region Private Helper Methods
        private UserDto CreateApplicationUserDto(User user)
        {
            return new UserDto
            {
                FirstName = user.FirsName,
                LastName = user.LastName,
                JWT = _jwtService.CreateJWT(user),

            };
        }

        private async Task<bool> CheckEmailExitsAsync(string email)
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email.ToLower());
        }
        #endregion

    }
}
