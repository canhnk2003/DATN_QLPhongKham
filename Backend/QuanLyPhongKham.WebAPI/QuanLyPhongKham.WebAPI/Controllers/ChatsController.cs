using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Business.Services;
using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Models;
using System.Security.Claims;

namespace QuanLyPhongKham.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly IGeminiService _geminiService;
        private readonly IMemoryCache _cache;
        private readonly IAppointmentService _appointmentService;
        private readonly IMapper _mapper;
        private readonly IPatientService _petientService;
        public ChatsController(IGeminiService geminiService, IMemoryCache cache, IAppointmentService appointmentService, IMapper mapper, IPatientService patientService)
        {
            _geminiService = geminiService;
            _cache = cache;
            _appointmentService = appointmentService;
            _mapper = mapper;
            _petientService = patientService;
        }
        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            if (string.IsNullOrEmpty(request.Message))
            {
                return BadRequest("Tin nhắn không được để trống!");
            }

            //Lấy userId từ JWT Token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Không xác định được người dùng!");
            }

            // Lấy session từ MemoryCache dựa vào UserId
            var sessionKey = $"ChatSession_{userId}";
            var context = _cache.Get<ChatContext>(sessionKey) ?? new ChatContext();

            // Gửi câu hỏi lên AI với ngữ cảnh
            var response = await _geminiService.GetResponseFromGemini(request.Message, context);

            if (request.Message.Trim().ToLower().Equals("đồng ý") && context.IsReadyForBooking)
            {
                if (context.DoctorId == Guid.Empty || context.ServiceId == Guid.Empty || context.DoctorId == null || context.ServiceId == null||
                    string.IsNullOrEmpty(context.PatientName) || string.IsNullOrEmpty(context.PatientPhone) ||
                    string.IsNullOrEmpty(context.PatientEmail) || context.AppointmentDate == default || context.AppointmentDate <= DateTime.Now ||
                    string.IsNullOrEmpty(context.AppointmentTime))
                {
                    return BadRequest("Thông tin đăng ký lịch khám không phù hợp (bạn kiểm tra lại ngày khám, ca khám và dịch vụ khám của bác sĩ) hoặc bị thiếu.");
                }
                int count = 0;
                var benhNhans = await _petientService.GetAllAsync();
                var benhNhan = benhNhans.FirstOrDefault(b => b.UserId == userId);
                if (benhNhan != null)
                {
                    benhNhan.HoTen = context.PatientName;
                    benhNhan.SoDienThoai = context.PatientPhone;
                    benhNhan.Email = context.PatientEmail;
                    int res = await _appointmentService.AddAsync(_mapper.Map<LichKham>(new AppointmentModel
                    {
                        BenhNhan = benhNhan,
                        BenhNhanId = benhNhan.BenhNhanId,
                        NgayKham = context.AppointmentDate,
                        GioKham = context.AppointmentTime,
                        BacSiId = context.DoctorId.Value,
                        DichVuId = context.ServiceId
                    }));
                    count = res;
                    await _petientService.UpdateAsync(benhNhan);
                }
                context.IsReadyForBooking = false;
                if (count > 0)
                {
                    response += "\n✅ Lịch khám đã được đặt thành công!";
                }
                else
                {
                    response += "\n❌ Có lỗi xảy ra khi đặt lịch. Vui lòng thử lại!";
                }
            }

            // Lưu lại session (cập nhật context)
            _cache.Set(sessionKey, context, TimeSpan.FromDays(3));

            return Ok(response);
        }
    }
}
