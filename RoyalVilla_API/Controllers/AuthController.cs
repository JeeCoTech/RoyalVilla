using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoyalVilla.API.Data;
using RoyalVilla.DTO;
using RoyalVilla.API.Services;

namespace RoyalVilla.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("register")]
        [ProducesResponseType(typeof(APIResponse<UserDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<APIResponse<UserDTO>>> RegisterUser([FromBody]RegistrationRequestDTO registrationRequestDTO)
        {
            try
            {
                if (registrationRequestDTO == null)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Registartion Data data is required"));
                }

                if (await _authService.IsEmailExistsAsync(registrationRequestDTO.Email))
                {
                    return Conflict(APIResponse<object>.Conflict($"Email with name {registrationRequestDTO.Email} already exists"));
                }

                var user = await _authService.RegisterAsync(registrationRequestDTO);

                if (user == null)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Registration failed"));
                }

                var response = APIResponse<UserDTO>.Ok(user, "User Created successfully");
                return CreatedAtAction(nameof(RegisterUser), response);
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(500, "An Error occured during Registration", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }


        [HttpPost("login")]
        [ProducesResponseType(typeof(APIResponse<LoginResponseDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse<LoginResponseDTO>>> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            try
            {
                if (loginRequestDTO == null)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Login data is required"));
                }

                var loginResponse = await _authService.LoginAsync(loginRequestDTO);

                if (loginResponse == null)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Login failed"));
                }

                var response = APIResponse<LoginResponseDTO>.Ok(loginResponse, "Login Successful");
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(500, "An Error occured during Login", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }
    }
}
