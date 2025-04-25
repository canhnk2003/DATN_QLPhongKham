using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Business.Services;

namespace QuanLyPhongKham.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomesController : ControllerBase
    {
        private readonly IHomeService _homeService;
        private readonly IServiceRatingService _ratingService;

        public HomesController(IHomeService homeService, IServiceRatingService ratingService)
        {
            _homeService = homeService;
            _ratingService = ratingService;
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetInfor()
        {
            var infor = await _homeService.GetClinicInfor();
            return Ok(infor);
        }
        
        [HttpGet("admin")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllDoctorInfor()
        {
            var infor = await _ratingService.GetAllAverageAsync();
            return Ok(infor);
        }

    }
}
