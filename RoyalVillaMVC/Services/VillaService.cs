using AutoMapper.Internal;
using RoyalVilla.DTO;
using RoyalVilla.Web.Extensions;
using RoyalVilla.Web.Models;
using RoyalVilla.Web.Services.IServices;
using RoyalVillaWeb.Models;
using RoyalVillaWeb.Services.IServices;

namespace RoyalVilla.Web.Services
{
    public class VillaService: BaseService, IVillaService
    {
        private const string APIEndpoint = $"/api/{SD.CurrentAPIVersion}/villa";
        public VillaService(IHttpClientFactory httpClient, IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration, ITokenProvider tokenProvider)
            : base(httpClient, tokenProvider, httpContextAccessor)
        {
        }

        public Task<T?> CreateAsync<T>(VillaCreateDTO dto)
        {

            var formData = dto.ToMultipartFormData();
            return SendAsync<T>(new APIRequest
            {
                ApiType = SD.APIType.POST,
                Data = formData,
                Url = APIEndpoint
            });
        }

        public Task<T?> DeleteAsync<T>(int id)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = SD.APIType.DELETE,
                Url = $"{APIEndpoint}/{id}"
            });
        }

        public Task<T?> GetAllAsync<T>()
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = SD.APIType.GET,
                Url = $"{APIEndpoint}"
            });
        }

        public Task<T?> GetAsync<T>(int id)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = SD.APIType.GET,

                Url = $"{APIEndpoint}/{id}"
            });
        }

        public Task<T?> UpdateAsync<T>(VillaUpdateDTO dto)
        {
            var formData = dto.ToMultipartFormData();
            return SendAsync<T>(new APIRequest
            {
                ApiType = SD.APIType.PUT,
                Data = formData,
                Url = $"{APIEndpoint}/{dto.Id}"
            });
        }
    }
}
