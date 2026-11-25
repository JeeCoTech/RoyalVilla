using RoyalVilla.DTO;
using RoyalVilla.Web.Models;
using RoyalVilla.Web.Services.IServices;

namespace RoyalVilla.Web.Services
{
    public class VillaService: BaseService, IVillaService
    {
        private readonly IHttpClientFactory _clientFactory;
        private const string APIEndpoint = "/api/villa";
        public VillaService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public async Task<T?> CreateAsync<T>(VillaCreateDTO dto, string token)
        {
            return await SendAsync<T>(new APIRequest()
            {
                APIType = SD.APIType.POST,
                Url = $"{APIEndpoint}",
                Data = dto,
                Token = token
            });
        }
        public async Task<T?> DeleteAsync<T>(int id, string token)
        {
            return await SendAsync<T>(new APIRequest()
            {
                APIType = SD.APIType.DELETE,
                Url = $"{APIEndpoint}/{id}",
                Token = token
            });
        }
        public async Task<T?> GetAllAsync<T>(string token)
        {
            return await SendAsync<T>(new APIRequest()
            {
                APIType = SD.APIType.GET,
                Url = $"{APIEndpoint}",
                Token = token
            });
        }
        public async Task<T?> GetAsync<T>(int id, string token)
        {
            return await SendAsync<T>(new APIRequest()
            {
                APIType = SD.APIType.GET,
                Url = $"{APIEndpoint}/{id}",
                Token = token
            });
        }
        public async Task<T?> UpdateAsync<T>(VillaUpdateDTO dto, string token)
        {
            return await SendAsync<T>(new APIRequest()
            {
                APIType = SD.APIType.PUT,
                Url = $"{APIEndpoint}/{dto.Id}",
                Data = dto,
                Token = token
            });
        }
    }
}
