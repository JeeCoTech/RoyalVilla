using System.ComponentModel.DataAnnotations;

namespace RoyalVilla_API.DTO
{
    public class LoginResponseDTO
    {
        [Required]
        public  string? Token { get; set; }

        public UserDTO? UserDTO { get; set; }
    }
}
