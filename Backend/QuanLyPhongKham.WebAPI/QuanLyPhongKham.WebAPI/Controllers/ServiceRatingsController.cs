using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Models;

namespace QuanLyPhongKham.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
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
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var ratings = await _serviceRatingService.GetAllWithNameAsync();
            return Ok(ratings);
        }
        [HttpGet("average")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllWithAverage()
        {
            var ratings = await _serviceRatingService.GetAllAverageAsync();
            return Ok(ratings);
        }
        [HttpGet("doctor/{doctorId}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllByDoctor(Guid doctorId)
        {
            var ratings = await _serviceRatingService.GetAllWithNameAsync();
            var ratingsByDoctor = ratings.Where(r => r.BacSiId == doctorId);
            return Ok(ratingsByDoctor);
        }
        [HttpGet("average/{doctorId}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetWithAverageByDoctor(Guid doctorId)
        {
            var rating = await _serviceRatingService.GetRatingByDoctor(doctorId);
            return Ok(rating);
        }

        [HttpGet("{DanhGiaId}")]
        public async Task<IActionResult> GetRatingById(Guid DanhGiaId)
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

        [Authorize(Roles = "Patient")]
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
