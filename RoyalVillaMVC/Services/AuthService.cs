using AutoMapper.Internal;
using RoyalVilla.DTO;
using RoyalVilla.Web;
using RoyalVilla.Web.Services;
using RoyalVilla.Web.Models;
using RoyalVillaWeb.Services.IServices;

namespace RoyalVillaWeb.Services
{
    public class AuthService : BaseService, IAuthService
    {

        private const string APIEndpoint = "/api/auth";
        public AuthService(IHttpClientFactory httpClient, IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration, ITokenProvider tokenProvider)
            : base(httpClient, tokenProvider, httpContextAccessor)
        {
        }

        public Task<T?> LoginAsync<T>(LoginRequestDTO loginRequestDTO)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = SD.APIType.POST,
                Data = loginRequestDTO,
                Url = APIEndpoint + "/login",
            }, withBearer: false);
        }

        public Task<T?> RefreshTokenAsync<T>(RefreshTokenRequestDTO refreshTokenRequestDTO)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = SD.APIType.POST,
                Data = refreshTokenRequestDTO,
                Url = APIEndpoint + "/refresh-token",
            }, withBearer: false);
        }

        public Task<T?> RegisterAsync<T>(RegistrationRequestDTO registerationRequestDTO)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = SD.APIType.POST,
                Data = registerationRequestDTO,
                Url = APIEndpoint + "/register",
            }, withBearer: false);
        }
    }
}
