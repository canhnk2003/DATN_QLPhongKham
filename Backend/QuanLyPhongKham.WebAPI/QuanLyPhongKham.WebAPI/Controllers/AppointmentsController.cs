using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Models;
using QuanLyPhongKham.Models.Resources;
using QuanLyPhongKham.Models.Exceptions;
using System.Security.Claims;

namespace QuanLyPhongKham.WebAPI.Controllers
{
    //[Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]

    public class AppointmentsController : ControllerBase
    {
        /// <summary>
        /// Quản lý lịch khám
        /// Created by: NKCanh - 05/11/2024
        /// </summary>
        private readonly IAppointmentService _appointmentService;
        private readonly IPatientService _patientService;
        private readonly IServiceRatingService _ratingService;
        private readonly IEmailService _emailService;
        private readonly IDoctorService _doctorService;
        private readonly IServiceService _serviceService;
        private readonly IMapper _mapper;

        public AppointmentsController(IAppointmentService appointmentService, IMapper mapper, IPatientService patientService, IServiceRatingService serviceRatingService, IEmailService emailService, IDoctorService doctorService, IServiceService serviceService)
        {
            _appointmentService = appointmentService;
            _mapper = mapper;
            _patientService = patientService;
            _ratingService = serviceRatingService;
            _emailService = emailService;
            _doctorService = doctorService;
            _serviceService = serviceService;
        }
        /// <summary>
        /// Lấy ra danh sách lịch khám
        /// </summary>
        /// Status code
        /// 200 - Lấy thành công
        /// <returns>DS lịch khám</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var appoinments = await _appointmentService.GetAppointment();
            return Ok(appoinments);
        }
        /// <summary>
        /// Lấy ra lịch khám theo id
        /// </summary>
        /// <param name="LichKhamId">id</param>
        /// <returns>lịch khám theo id</returns>
        [HttpGet("{LichKhamId}")]
        public async Task<IActionResult> GetAppointmentById(Guid LichKhamId)
        {
            var appointments = await _appointmentService.GetAppointment();
            var appointment = appointments.FirstOrDefault(x => x.LichKhamId == LichKhamId);
            return Ok(appointment);
        }
        /// <summary>
        /// Đăng ký 1 lịch khám
        /// </summary>
        /// <param name="lichKham">dữ liệu</param>
        /// <returns>
        /// 201 - Tạo thành công
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Post([FromBody] AppointmentModel lichKham)
        {
            BenhNhan benhNhan = await _patientService.GetByIdAsync(lichKham.BenhNhanId);
            if (!string.IsNullOrEmpty(lichKham.BenhNhan.HoTen))
                benhNhan.HoTen = lichKham.BenhNhan.HoTen;
            if (!string.IsNullOrEmpty(lichKham.BenhNhan.NgaySinh.ToString()))
                benhNhan.NgaySinh = lichKham.BenhNhan.NgaySinh;
            if (!string.IsNullOrEmpty(lichKham.BenhNhan.Email))
                benhNhan.Email = lichKham.BenhNhan.Email;
            if (!string.IsNullOrEmpty(lichKham.BenhNhan.SoDienThoai))
                benhNhan.SoDienThoai = lichKham.BenhNhan.SoDienThoai;
            if (!string.IsNullOrEmpty(lichKham.BenhNhan.TienSuBenhLy))
                benhNhan.TienSuBenhLy = lichKham.BenhNhan.TienSuBenhLy;
            if (!string.IsNullOrEmpty(lichKham.BenhNhan.DiaChi))
                benhNhan.DiaChi = lichKham.BenhNhan.DiaChi;
            if (!string.IsNullOrEmpty(lichKham.BenhNhan.LoaiGioiTinh.ToString()))
                benhNhan.LoaiGioiTinh = lichKham.BenhNhan.LoaiGioiTinh;

            lichKham.BenhNhan = benhNhan;
            int res = await _appointmentService.AddAsync(_mapper.Map<LichKham>(lichKham));
            await _patientService.UpdateAsync(benhNhan);

            var doctor = await _doctorService.GetByIdAsync(lichKham.BacSiId);
            var service = await _serviceService.GetByIdAsync((Guid)lichKham.DichVuId);

            var body = $@"
                <html lang='vi'>
                <head>
                    <meta charset='UTF-8'>
                    <title>Xác Nhận Đặt Lịch Khám</title>
                    <style>
                        body {{background-color: #f4f4f9; color: #333; padding: 20px; }}
                        .container {{ width: 100%; max-width: 600px; margin: 0 auto; background-color: #fff; padding: 20px; border-radius: 8px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1); }}
                        .header {{ background-color: #007bff; color: #fff; text-align: center; padding: 15px; border-radius: 8px 8px 0 0; }}
                        .content {{ margin-top: 20px; }}
                        .footer {{ margin-top: 30px; text-align: center; font-size: 14px; background-color: #f9f9f9; padding: 15px; border-radius: 0 0 8px 8px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2>Xác nhận Đặt lịch khám</h2>
                        </div>
                        <div class='content'>
                            <p>Xin chào {lichKham.BenhNhan.HoTen},</p>
                            <p>Cảm ơn bạn đã đặt lịch khám tại phòng khám của chúng tôi. Dưới đây là thông tin lịch khám của bạn:</p>
                            <table>
                                <tr><td><strong>Ngày khám:</strong></td><td>{lichKham.NgayKham:dd/MM/yyyy}</td></tr>
                                <tr><td><strong>Giờ khám:</strong></td><td>{lichKham.GioKham}</td></tr>
                                <tr><td><strong>Bác sĩ:</strong></td><td>{doctor.HoTen}</td></tr>
                                <tr><td><strong>Dịch vụ:</strong></td><td>{service.TenDichVu}</td></tr>
                            </table>
                            <p>Chúng tôi sẽ chuẩn bị tốt nhất để đón tiếp bạn vào đúng giờ. Hẹn gặp lại bạn tại phòng khám!</p>
                        </div>
                        <div class='footer'>
                            <p>Phòng Khám ABC</p>
                            <p><a href='http://127.0.0.1:5500/User/index.html'>Truy cập trang web của chúng tôi</a></p>
                        </div>
                    </div>
                </body>
                </html>
                ";

            await _emailService.SendEmailAsync(lichKham.BenhNhan.Email, "Xác nhận Đặt lịch khám", body);
            return Ok(new
            {
                statusCode = 201,
                response = res,
                message = "Đặt lịch thành công và đã gửi email xác nhận."
            });
        }
        /// <summary>
        /// Sửa 1 lịch khám theo id
        /// </summary>
        /// <param name="LichKhamId">id</param>
        /// <param name="lichKham">dữ liệu</param>
        /// <returns>201 - Lấy thành công</returns>
        [HttpPut("{LichKhamId}")]
        public async Task<IActionResult> Update(Guid LichKhamId, [FromBody] AppointmentModel lichKham)
        {
            LichKham appointment = await _appointmentService.GetByIdAsync(LichKhamId);
            BenhNhan benhNhan = await _patientService.GetByIdAsync(lichKham.BenhNhanId);
            appointment.BacSiId = lichKham.BacSiId;
            appointment.NgayKham = lichKham.NgayKham;
            appointment.GioKham = lichKham.GioKham;
            appointment.TrangThaiLichKham = "Đang xử lý";
            appointment.DichVuId = lichKham.DichVuId;
            if (!string.IsNullOrEmpty(lichKham.BenhNhan.HoTen))
                benhNhan.HoTen = lichKham.BenhNhan.HoTen;
            if (!string.IsNullOrEmpty(lichKham.BenhNhan.NgaySinh.ToString()))
                benhNhan.NgaySinh = lichKham.BenhNhan.NgaySinh;
            if (!string.IsNullOrEmpty(lichKham.BenhNhan.Email))
                benhNhan.Email = lichKham.BenhNhan.Email;
            if (!string.IsNullOrEmpty(lichKham.BenhNhan.SoDienThoai))
                benhNhan.SoDienThoai = lichKham.BenhNhan.SoDienThoai;
            if (!string.IsNullOrEmpty(lichKham.BenhNhan.DiaChi))
                benhNhan.DiaChi = lichKham.BenhNhan.DiaChi;
            if (!string.IsNullOrEmpty(lichKham.BenhNhan.TienSuBenhLy))
                benhNhan.TienSuBenhLy = lichKham.BenhNhan.TienSuBenhLy;
            if (!string.IsNullOrEmpty(lichKham.BenhNhan.LoaiGioiTinh.ToString()))
                benhNhan.LoaiGioiTinh = lichKham.BenhNhan.LoaiGioiTinh;
            appointment.BenhNhan = benhNhan;
            appointment.NgayCapNhat = DateTime.Now;
            int res = await _appointmentService.EditAsync(appointment, LichKhamId);
            await _patientService.UpdateAsync(benhNhan);
            return StatusCode(201, res);

        }
        /// <summary>
        /// Hủy 1 lịch khám theo id
        /// </summary>
        /// <param name="LichKhamId">id</param>
        /// <returns>
        /// 201 - Hủy thành công
        /// 400 - Lỗi hủy
        /// </returns>
        [HttpPut("cancel/{LichKhamId}")]
        [Authorize(Roles = "Patient,Doctor")]
        public async Task<IActionResult> Cancel(Guid LichKhamId, [FromBody] string? lyDo)
        {
            int res = await _appointmentService.CancelAppointment(LichKhamId, lyDo);
            return StatusCode(201, res);
        }
        [HttpPut("doctor/{LichKhamId}")]
        public async Task<IActionResult> AcceptAppointment(Guid LichKhamId)
        {
            int res = await _appointmentService.AcceptAppointment(LichKhamId);
            return StatusCode(201, res);
        }
        [HttpPut("appointment/{LichKhamId}")]
        public async Task<IActionResult> CompleteAppointment(Guid LichKhamId)
        {
            int res = await _appointmentService.CompleteAppointment(LichKhamId);
            return StatusCode(201, res);
        }

        /// <summary>
        /// Xóa lịch khám theo id
        /// </summary>
        /// <param name="LichKhamId">id</param>
        /// <returns>201 - Xóa thành công</returns>
        [HttpDelete("{LichKhamId}")]
        [Authorize(Roles = "Admin,Patient")]
        public async Task<IActionResult> Delete(Guid LichKhamId)
        {
            var relatedReviews = await _ratingService.GetAllAsync();
            relatedReviews = relatedReviews.Where(r => r.LichKhamId == LichKhamId).ToList();
            int count = 0;
            foreach (var relatedReview in relatedReviews)
            {
                relatedReview.LichKhamId = null;
                await _ratingService.UpdateAsync(relatedReview);
                count++;
            }
            int res = await _appointmentService.DeleteAsync(LichKhamId);
            return StatusCode(201, res);
        }
        /// <summary>
        /// Lấy ra danh sách lịch khám theo bacSiId
        /// </summary>
        /// <param name="bacSiId">id</param>
        /// <returns>200 - OK</returns>
        [HttpGet("doctor/{DoctorId}")]
        public async Task<IActionResult> GetAppointmentsByDoctor(Guid DoctorId)
        {
            var lichKhams = await _appointmentService.GetAppointment();
            lichKhams = lichKhams.Where(x => x.BacSiId == DoctorId);
            return Ok(lichKhams);
        }
        [HttpGet("patient/{PatientId}")]
        public async Task<IActionResult> GetAppointmentsByPatient(Guid PatientId)
        {
            var lichKhams = await _appointmentService.GetAppointment();
            lichKhams = lichKhams.Where(x => x.BenhNhanId == PatientId);
            return Ok(lichKhams);
        }
        [HttpGet("appointment/{PatientId}")]
        public async Task<IActionResult> GetAppointmentLatest(Guid PatientId)
        {
            var lichKham = await _appointmentService.GetLichKhamLatest(PatientId);
            return Ok(_mapper.Map<AppointmentModel>(lichKham));
        }

    }
}
