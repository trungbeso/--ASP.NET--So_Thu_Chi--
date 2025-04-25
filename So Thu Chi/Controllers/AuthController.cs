using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using So_Thu_Chi.Dtos;
using So_Thu_Chi.Models;
using So_Thu_Chi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace So_Thu_Chi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register (UserDto request)
        {
            var newUser = await authService.RegisterAsync(request);
            if (newUser == null)
            {
                return BadRequest("User already exists.");
            }
            return Ok(newUser);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
        {
            var result = await authService.LoginAsync(request);
            if (result == null)
            {
                return BadRequest("Invalid username or password.");
            }
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto req)
        {
            var result = await authService.RefreshTokenAsync(req);
            if (result == null || result.AccessToken is null || result.RefreshToken is null)
            {
                return BadRequest("Invalid refresh token.");
            }
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        public IActionResult AuthenticateOnlyEndpoint()
        {
            return Ok("You are authenticated.");
        }

        [HttpGet("admin-only")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("You are Admin.");
        }
    }
}
