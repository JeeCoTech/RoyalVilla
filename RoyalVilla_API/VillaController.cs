using Microsoft.AspNetCore.Mvc;

namespace RoyalVilla_API
{
    [Route("api/villa")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        [HttpGet]  
        public string GetVillas()
        {
            return "Get All Villas";
        }
    }
}
