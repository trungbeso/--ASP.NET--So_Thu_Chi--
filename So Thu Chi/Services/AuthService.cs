using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using So_Thu_Chi.Dtos;
using So_Thu_Chi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace So_Thu_Chi.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<TokenResponseDto?> LoginAsync(UserDto request)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Username == request.Username);
            if (existingUser == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            if (new PasswordHasher<User>().VerifyHashedPassword(existingUser, existingUser.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                throw new InvalidOperationException("Wrong password.");
            }

            return await GenerateTokenResponse(existingUser);
        }

        public async Task<User?> RegisterAsync(UserDto request)
        {
            if (await _context.Users.AnyAsync(x => x.Username == request.Username))
            {
                throw new InvalidOperationException("User already exists.");
            }

            User newUser = new User { Username = request.Username };
            var hashedPassword = new PasswordHasher<User>().HashPassword(newUser, request.Password);
            newUser.PasswordHash = hashedPassword;

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto req)
        {
            var user = await ValicateRefreshToken(req.UserId, req.RefreshTokne);
            if (user == null)
            {
                throw new InvalidOperationException("Invalid refresh token.");
            }
            return await GenerateTokenResponse(user);
        }

        private async Task<TokenResponseDto> GenerateTokenResponse(User user)
        {
            return new TokenResponseDto
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };
        }

        private async Task<User> ValicateRefreshToken(Guid id, string refreshToken)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenEnpiryTime <= DateTime.UtcNow)
            {
                throw new InvalidOperationException("Invalid refresh token.");
            }
            return user;
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user) {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenEnpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();
            return refreshToken;
        }

        private string GenerateRefreshToken()
        {
            var dandomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(dandomNumber);
            return Convert.ToBase64String(dandomNumber);
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var tokenKey = _config.GetValue<string>("AppSettings:Token");
            if (string.IsNullOrEmpty(tokenKey))
            {
                throw new InvalidOperationException("JWT Token configuration ('AppSettings:Token') is missing.");
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var issuer = _config.GetValue<string>("Applications:Issuer") ?? _config.GetValue<string>("Applications:Issuer");
            var audience = _config.GetValue<string>("Applications:Audience") ?? _config.GetValue<string>("Applications:Audience");

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
