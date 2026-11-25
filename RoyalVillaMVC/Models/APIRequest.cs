using Microsoft.AspNetCore.Mvc;
using static RoyalVilla.Web.SD;

namespace RoyalVilla.Web.Models
{
    public class APIRequest
    {
        public APIType APIType { get; set; } = APIType.GET;
        public string? Url { get; set; }
        public object? Data { get; set; }
        public string? Token { get; set; }
    }
}
