using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoyalVilla.API.Data;
using RoyalVilla.DTO;
using RoyalVilla.API.Services;
using RoyalVilla.API.Services.IServices;

namespace RoyalVilla.API.Controllers
{
    [Route("api/auth")]
    [ApiVersionNeutral]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("register")]
        [ProducesResponseType(typeof(APIResponse<UserDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<APIResponse<UserDTO>>> Register([FromBody] RegistrationRequestDTO registerationRequestDTO)
        {
            try
            {
                if (registerationRequestDTO == null)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Registeration data is required"));
                }

                if (await _authService.IsEmailExistsAsync(registerationRequestDTO.Email))
                {
                    return Conflict(APIResponse<object>.Conflict($"User with email '{registerationRequestDTO.Email}' already exists"));
                }

                var user = await _authService.RegisterAsync(registerationRequestDTO);

                if (user == null)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Registration failed"));
                }

                //auth service 
                var response = APIResponse<UserDTO>.CreatedAt(user, "User registered successfully");
                return CreatedAtAction(nameof(Register), response);
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(500, "An error occurred during registration", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }



        [HttpPost("login")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<TokenDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse<TokenDTO>>> Login([FromBody] LoginRequestDTO loginRequestDTO)
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
                    return BadRequest(APIResponse<object>.BadRequest("Login failed. Please check your credentials."));
                }

                //auth service 
                var response = APIResponse<TokenDTO>.Ok(loginResponse, "Login successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(500, "An error occurred during login", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }


        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(APIResponse<UserDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<APIResponse<UserDTO>>> RefreshAccessToken([FromBody] RefreshTokenRequestDTO refreshTokenRequestDTO)
        {
            try
            {
                if (refreshTokenRequestDTO == null || String.IsNullOrEmpty(refreshTokenRequestDTO.RefreshToken))
                {
                    return BadRequest(APIResponse<object>.BadRequest("Refresh Token is required"));
                }

                var tokenResponse = await _authService.RefreshAccessTokenAsync(refreshTokenRequestDTO);

                if (tokenResponse == null)
                {
                    // Token reuse or invalid token - log for security monitoring
                    var errorResponse = APIResponse<object>.Error(
                       401,
                       "Invalid or expired refresh token. If token reuse was detected, all your sessions have been terminated for security. Please login again.");
                    return Unauthorized(errorResponse);
                }


                //auth service 
                var response = APIResponse<TokenDTO>.Ok(tokenResponse, "Token refreshed successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(500, "An error occurred during token refresh", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }


    }
}
