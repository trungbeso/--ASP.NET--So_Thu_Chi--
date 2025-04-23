using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using So_Thu_Chi.Dtos;
using So_Thu_Chi.Models;
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

        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        public ActionResult<User> Register (UserDto request)
        {
            User newUser = new User { Username = request.Username };
            var hashedPassword = new PasswordHasher<User>().HashPassword(newUser, request.Password);
            newUser.PasswordHash = hashedPassword;

            return Ok(newUser);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Username == request.Username);
            if (existingUser == null)
            {
                return BadRequest("User not found.");
            }
            if (new PasswordHasher<User>().VerifyHashedPassword(existingUser, existingUser.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                return BadRequest("Wrong password.");
            }
            string token = CreateToken(existingUser);
            return Ok(token);
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var tokenKey = _configuration.GetValue<string>("AppSettings:Token");
            if (string.IsNullOrEmpty(tokenKey))
            {
                throw new InvalidOperationException("JWT Token configuration ('AppSettings:Token') is missing.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var issuer = _configuration.GetValue<string>("Applications:Issuer") ?? _configuration.GetValue<string>("Applications:Issuer");
            var audience = _configuration.GetValue<string>("Applications:Audience") ?? _configuration.GetValue<string>("Applications:Audience");

            var tokenDescription = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescription);
        }
    }
}
