using So_Thu_Chi.Dtos;
using So_Thu_Chi.Models;

namespace So_Thu_Chi.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto request);
        Task<TokenResponseDto?> LoginAsync(UserDto request);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto req);
    }
}
