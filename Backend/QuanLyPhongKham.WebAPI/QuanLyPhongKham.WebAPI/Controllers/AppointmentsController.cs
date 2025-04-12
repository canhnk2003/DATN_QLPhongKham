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
        private readonly IMapper _mapper;

        public AppointmentsController(IAppointmentService appointmentService, IMapper mapper, IPatientService patientService)
        {
            _appointmentService = appointmentService;
            _mapper = mapper;
            _patientService = patientService;
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
            var appoinments = await _appointmentService.GetAllAsync();
            appoinments = appoinments.OrderByDescending(l => l.NgayCapNhat).ToList();
            return Ok(_mapper.Map<IEnumerable<AppointmentModel>>(appoinments));
        }
        /// <summary>
        /// Lấy ra lịch khám theo id
        /// </summary>
        /// <param name="LichKhamId">id</param>
        /// <returns>lịch khám theo id</returns>
        [HttpGet("{LichKhamId}")]
        public async Task<IActionResult> GetAppointmentById(Guid LichKhamId)
        {
            var appointment = await _appointmentService.GetByIdAsync(LichKhamId);
            return Ok(_mapper.Map<AppointmentModel>(appointment));
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
            benhNhan.HoTen = lichKham.BenhNhan.HoTen;
            benhNhan.NgaySinh = lichKham.BenhNhan.NgaySinh;
            benhNhan.Email = lichKham.BenhNhan.Email;
            benhNhan.SoDienThoai = lichKham.BenhNhan.SoDienThoai;
            benhNhan.TienSuBenhLy = lichKham.BenhNhan.TienSuBenhLy;
            benhNhan.DiaChi = lichKham.BenhNhan.DiaChi;
            benhNhan.LoaiGioiTinh = lichKham.BenhNhan.LoaiGioiTinh;

            lichKham.BenhNhan = benhNhan;
            int res = await _appointmentService.AddAsync(_mapper.Map<LichKham>(lichKham));
            await _patientService.UpdateAsync(benhNhan);
            return StatusCode(201, res);
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
            benhNhan.HoTen = lichKham.BenhNhan.HoTen;
            benhNhan.NgaySinh = lichKham.BenhNhan.NgaySinh;
            benhNhan.Email = lichKham.BenhNhan.Email;
            benhNhan.SoDienThoai = lichKham.BenhNhan.SoDienThoai;
            benhNhan.DiaChi = lichKham.BenhNhan.DiaChi;
            benhNhan.TienSuBenhLy = lichKham.BenhNhan.TienSuBenhLy;
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
            var lichKhams = await _appointmentService.GetAppointmentsByDoctor(DoctorId);
            return Ok(_mapper.Map<IEnumerable<AppointmentModel>>(lichKhams));
        }
        [HttpGet("patient/{PatientId}")]
        public async Task<IActionResult> GetAppointmentsByPatient(Guid PatientId)
        {
            var lichKhams = await _appointmentService.GetAppointmentsByPatient(PatientId);
            lichKhams = lichKhams.OrderByDescending(l => l.NgayCapNhat).ToList();
            return Ok(_mapper.Map<IEnumerable<AppointmentModel>>(lichKhams));
        }
        [HttpGet("appointment/{PatientId}")]
        public async Task<IActionResult> GetAppointmentLatest(Guid PatientId)
        {
            var lichKham = await _appointmentService.GetLichKhamLatest(PatientId);
            return Ok(_mapper.Map<AppointmentModel>(lichKham));
        }

        [Authorize]
        [HttpGet("test-auth")]
        public IActionResult TestAuth()
        {
            var userName = User.Identity?.Name;
            var isAuth = User.Identity?.IsAuthenticated;
            var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

            return Ok(new { userName, isAuth, roles });
        }


    }
}
