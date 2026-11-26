using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RoyalVilla.DTO;
using RoyalVilla.Web.Services.IServices;
using RoyalVilla.Web.Models;
using RoyalVillaWeb.Models;
using System.Diagnostics;

namespace RoyalVillaMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;

        public HomeController(IVillaService villaService, IMapper mapper)
        {
            _mapper = mapper;
            _villaService = villaService;
        }

        public async Task<IActionResult> Index()
        {
            List<VillaDTO> villaList = new();
            try
            {
                var response = await _villaService.GetAllAsync<APIResponse<List<VillaDTO>>>();
                if (response != null && response.Success && response.Data != null)
                {
                    villaList = response.Data;
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred: {ex.Message}";
            }

            return View(villaList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
