using RoyalVilla.DTO;
using RoyalVilla.Web.Models;

namespace RoyalVilla.Web.Services.IServices
{
    public interface IBaseService
    {
        APIResponse<object> ResponseModel { get; set; }
        Task<T> SendAsync<T>(APIRequest apiRequest);
    }
}
