using RoyalVilla.DTO;
using RoyalVilla.Web.Models;
using RoyalVilla.Web.Services.IServices;
using System.Text.Json;
using static RoyalVilla.Web.SD;

namespace RoyalVilla.Web.Services
{
    public class BaseService : IBaseService
    {
       public IHttpClientFactory _httpClient { get; set; }

       private static readonly JsonSerializerOptions JsonOptions = new()
       {
           PropertyNameCaseInsensitive = true
       };

        public APIResponse<object> ResponseModel { get ; set; }

        public BaseService(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
            this.ResponseModel = new();
        }
        public async Task<T?> SendAsync<T>(APIRequest apiRequest)
        {
            try
            {
                var client = _httpClient.CreateClient("RoyalVillaAPI");
                var message = new HttpRequestMessage
                {
                    RequestUri = new Uri(apiRequest.Url, uriKind: UriKind.Relative),
                    Method = GetHttpMethod(apiRequest.APIType),
                };

                if(apiRequest.Data!= null)
                {
                    message.Content = JsonContent.Create(apiRequest.Data, options: JsonOptions);
                }

                var apiResponse = await client.SendAsync(message);
                return await apiResponse.Content.ReadFromJsonAsync<T>(JsonOptions);
            }
            catch (Exception ex)
            {

                Console.WriteLine("An unexpected error occured during API request." + ex.Message);
                return default;
            }
        }

        private static HttpMethod GetHttpMethod(APIType apiType)
        {
            return apiType switch
            {
                APIType.POST => HttpMethod.Post,
                APIType.PUT => HttpMethod.Put,
                APIType.DELETE => HttpMethod.Delete,
                _ => HttpMethod.Get,
            };
        }
    }
}
