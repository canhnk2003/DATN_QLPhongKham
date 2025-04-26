using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Business.Services;
using QuanLyPhongKham.Models.Models;

namespace QuanLyPhongKham.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class HomesController : ControllerBase
    {
        private readonly IHomeService _homeService;
        private readonly IServiceRatingService _ratingService;
        private readonly IAppointmentService _appointmentService;

        public HomesController(IHomeService homeService, IServiceRatingService ratingService, IAppointmentService appointmentService)
        {
            _homeService = homeService;
            _ratingService = ratingService;
            _appointmentService = appointmentService;
        }
        /// <summary>
        /// Lấy thông tin phòng khám
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetInfor()
        {
            var infor = await _homeService.GetClinicInfor(null);
            return Ok(infor);
        }

        [HttpGet("{doctorId}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetInforByDoctor(Guid doctorId)
        {
            var infor = await _homeService.GetClinicInfor(doctorId);
            return Ok(infor);
        }

        /// <summary>
        /// Lấy thông tin lịch khám theo ngày hiện tại
        /// </summary>
        /// <returns></returns>
        [HttpGet("appointment")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAppointmentToday()
        {
            var appointments = await _appointmentService.GetAppointment();
            var appointmentTodays = appointments.Where(x => x.NgayKham.HasValue && x.NgayKham.Value.Date == DateTime.Today).ToList();
            return Ok(appointmentTodays);
        }

        [HttpGet("appointment/{doctorId}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAppointmentTodayByDoctor(Guid doctorId)
        {
            var appointments = await _appointmentService.GetAppointment();
            var appointmentTodays = appointments.Where(x => x.BacSiId == doctorId && x.NgayKham.HasValue && x.NgayKham.Value.Date == DateTime.Today).ToList();
            return Ok(appointmentTodays);
        }

        /// <summary>
        /// Lấy các bác sĩ với đánh giá
        /// </summary>
        /// <returns></returns>
        [HttpGet("admin")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllDoctorInfor()
        {
            var infor = await _ratingService.GetAllAverageAsync();
            return Ok(infor);
        }

        /// <summary>
        /// Thống kê lịch khám theo năm
        /// </summary>
        /// <returns></returns>
        [HttpGet("statisticByYear")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> ThongKeLichKhamTheoNamAsys()
        {
            var statistic = await _homeService.ThongKeLichKhamTheoNamAsync(null);
            return Ok(statistic);
        }

        [HttpGet("statisticByYear/{doctorId}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> ThongKeLichKhamTheoNamByDoctorAsys(Guid doctorId)
        {
            var statistic = await _homeService.ThongKeLichKhamTheoNamAsync(doctorId);
            return Ok(statistic);
        }

        /// <summary>
        /// Thống kê lịch khám theo trạng thái
        /// </summary>
        /// <returns></returns>
        [HttpGet("statisticByStatus")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> ThongKeLichKhamTheoTrangThaiAsys()
        {
            var statistic = await _homeService.ThongKeLichKhamTheoTrangThaiAsync(null);
            return Ok(statistic);
        }

        [HttpGet("statisticByStatus/{doctorId}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> ThongKeLichKhamTheoTrangThaiByDoctorAsys(Guid doctorId)
        {
            var statistic = await _homeService.ThongKeLichKhamTheoTrangThaiAsync(doctorId);
            return Ok(statistic);
        }

        /// <summary>
        /// Thống kê dịch vụ khám phổ biến
        /// </summary>
        /// <returns></returns>
        [HttpGet("statisticServicePopular")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> ThongKeDichvuPhoBienAsysn()
        {
            var statistic = await _homeService.ThongKeDichVuPhoBienTheoNamAsync();
            return Ok(statistic);
        }

        [HttpGet("statisticRating/{doctorId}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> ThongKeDanhGiaAsysn(Guid doctorId)
        {
            var statistic = await _ratingService.GetAllWithNameAsync();
            int total = statistic.Count();
            var grouped = statistic
                .GroupBy(x => x.DanhGia)
                .Select(g => new DanhGiaPhanTramModel
                {
                    DanhGia = $"{g.Key} sao",
                    PhanTram = Math.Round((double)g.Count() * 100 / total, 2)
                })
                .ToList();
            // Đảm bảo đủ từ 1 đến 5 sao (kể cả khi không có đánh giá)
            var fullResult = Enumerable.Range(1, 5)
                .Select(i => grouped.FirstOrDefault(x => x.DanhGia == $"{i} sao")
                    ?? new DanhGiaPhanTramModel { DanhGia = $"{i} sao", PhanTram = 0 })
                .ToList();
            return Ok(fullResult);
        }
    }
}
