using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Models;

namespace QuanLyPhongKham.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceRatingsController : ControllerBase
    {
        private readonly IServiceRatingService _serviceRatingService;
        private readonly IMapper _mapper;

        public ServiceRatingsController(IServiceRatingService serviceRatingService, IMapper mapper)
        {
            _serviceRatingService = serviceRatingService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var ratings = await _serviceRatingService.GetAllAsync();
            ratings = ratings.OrderByDescending(l => l.NgayCapNhat).ToList();
            return Ok(_mapper.Map<IEnumerable<DanhGiaDichvuModel>>(ratings));
        }

        [HttpGet("{DanhGiaId}")]
        public async Task<IActionResult> GetAppointmentById(Guid DanhGiaId)
        {
            var rating = await _serviceRatingService.GetByIdAsync(DanhGiaId);
            return Ok(_mapper.Map<DanhGiaDichvuModel>(rating));
        }

        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Post([FromBody] DanhGiaDichvuModel danhgia)
        {
            int res = await _serviceRatingService.AddAsync(_mapper.Map<DanhGiaDichVu>(danhgia));
            return StatusCode(201, res);
        }

        [HttpPut("{DanhGiaId}")]
        public async Task<IActionResult> Update(Guid DanhGiaId, [FromBody] DanhGiaDichvuModel danhGia)
        {
            int res = await _serviceRatingService.EditAsync(_mapper.Map<DanhGiaDichVu>(danhGia), DanhGiaId);
            return StatusCode(201, res);
        }

        [HttpDelete("{DanhGiaId}")]
        [Authorize(Roles = "Admin,Patient")]
        public async Task<IActionResult> Delete(Guid DanhGiaId)
        {
            int res = await _serviceRatingService.DeleteAsync(DanhGiaId);
            return StatusCode(201, res);
        }
    }
}
