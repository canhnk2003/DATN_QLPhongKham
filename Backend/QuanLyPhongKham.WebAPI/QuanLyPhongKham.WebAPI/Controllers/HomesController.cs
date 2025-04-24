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

        public HomesController(IHomeService homeService)
        {
            _homeService = homeService;
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
            var infor = await _homeService.GetAllDoctorInfor();
            return Ok(infor);
        }

    }
}
