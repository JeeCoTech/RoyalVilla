using RoyalVilla.DTO;

namespace RoyalVillaWeb.Services.IServices
{
    public interface IAuthService
    {
        Task<T?> LoginAsync<T>(LoginRequestDTO loginRequestDTO);
        Task<T?> RegisterAsync<T>(RegistrationRequestDTO registerationRequestDTO);
        Task<T?> RefreshTokenAsync<T>(RefreshTokenRequestDTO refreshTokenRequestDTO);
        
    }
}
