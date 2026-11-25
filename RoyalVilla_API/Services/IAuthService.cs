using RoyalVilla_API.DTO;

namespace RoyalVilla_API.Services
{
    public interface IAuthService
    {
        Task<UserDTO?> RegisterAsync(RegistrationRequestDTO resgistrationRequestDTO);

        Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO);

        Task<bool> IsEmailExistsAsync(string email);


    }
}
