using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RoyalVilla.API.Data;
using RoyalVilla.DTO;
using RoyalVilla.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RoyalVilla.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext db, IConfiguration configuration ,IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _db.User.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            try 
            {

            var user = _db.User.FirstOrDefault(u => u.Email.ToLower() == loginRequestDTO.Email.ToLower() && u.Password == loginRequestDTO.Password);
            if(user == null || user.Password != loginRequestDTO.Password)
            {
                return null;
            }

                //generate JWT token

                var token = GenerateJwtToken(user);

                return new LoginResponseDTO
                {
                    UserDTO = _mapper.Map<UserDTO>(user),
                    Token = token
                };
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException("An unexpected error occured during user registration.");
    }
}

        public async Task<UserDTO?> RegisterAsync(RegistrationRequestDTO resgistrationRequestDTO)
        {
            try
            {
                if (await IsEmailExistsAsync(resgistrationRequestDTO.Email))
                {
                    throw new InvalidOperationException($"User with email {resgistrationRequestDTO.Email} already exists.");
                }

                User user = new()
                {
                    Email = resgistrationRequestDTO.Email,
                    Name = resgistrationRequestDTO.Name,
                    Password = resgistrationRequestDTO.Password,
                    Role = string.IsNullOrEmpty(resgistrationRequestDTO.Role) ? "Customer" : resgistrationRequestDTO.Role,
                    CreatedDate = DateTime.Now
                };

                await _db.User.AddAsync(user);
                await _db.SaveChangesAsync();

                return _mapper.Map<UserDTO>(user);
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException("An unexpected error occured during user registration.");
            }
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JwtSettings")["Secret"]);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                  new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                  new Claim(ClaimTypes.Email, user.Email),
                  new Claim(ClaimTypes.Name, user.Name),
                  new Claim(ClaimTypes.Role, user.Role),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescription);
            return tokenHandler.WriteToken(token);
        }
    }
}
