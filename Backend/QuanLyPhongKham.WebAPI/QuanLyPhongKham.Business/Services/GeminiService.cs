using Microsoft.Extensions.Options;
using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Data.Interfaces;
using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Business.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public GeminiService(IOptions<GoogleAISettings> settings, IServiceRepository serviceRepository, IDoctorRepository doctorRepository, IDepartmentRepository departmentRepository, IAppointmentRepository appointmentRepository)
        {
            _httpClient = new HttpClient();
            _apiKey = settings.Value.ApiKey;
            _serviceRepository = serviceRepository;
            _doctorRepository = doctorRepository;
            _departmentRepository = departmentRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<string> GetResponseFromGemini(string userInput, ChatContext context)
        {
            // Nếu người dùng xác nhận đặt lịch, tiến hành đặt lịch
            if (userInput.Contains("Đồng ý", StringComparison.OrdinalIgnoreCase))
            {
                if (context.IsReadyForBooking)
                {
                    return "Cảm ơn bạn! Tôi sẽ tiến hành đặt lịch ngay bây giờ.";
                }
                else
                {
                    return "Bạn chưa chọn lịch khám! Hãy cung cấp thông tin cần thiết trước.";
                }
            }


            // Lấy danh sách bác sĩ và dịch vụ khám từ database
            var doctors = await _doctorRepository.GetAllAsync();
            var services = await _serviceRepository.GetAllAsync();
            //Lấy ra danh sách lịch khám hiện tại
            var appointments = await _appointmentRepository.GetAllAsync();

            if (!doctors.Any())
            {
                return "Hiện tại chưa có dữ liệu về bác sĩ!";
            }
            if (!services.Any())
            {
                return "Hiện tại chưa có dữ liệu về dịch vụ khám!";
            }

            // Tạo nội dung gửi cho Gemini
            var prompt = new StringBuilder();
            prompt.AppendLine("Bạn là một trợ lý AI giúp bệnh nhân chọn bác sĩ, dịch vụ và lịch khám hợp lý.");

            //Thêm danh sách bác sĩ
            if (doctors.Any())
            {
                prompt.AppendLine("Dưới đây là danh sách bác sĩ trong hệ thống:\n");
                foreach (var doctor in doctors)
                {
                    var tenKhoa = doctor.Khoa != null ? doctor.Khoa.TenKhoa :
                        (doctor.KhoaId.HasValue ? (await _departmentRepository.GetByIdAsync(doctor.KhoaId.Value))?.TenKhoa : "Chưa cập nhật");

                    prompt.AppendLine($"- ID: {doctor.BacSiId}, Họ tên: {doctor.HoTen}, Bằng cấp: {doctor.TenBangCap}, Chuyên khoa: {tenKhoa}" +
                        $"Số điện thoại: {doctor.SoDienThoai}, Địa chỉ: {doctor.DiaChi}, Kinh nghiệm: {doctor.SoNamKinhNghiem} năm, " +
                        $"Giờ làm việc (là giờ có thể đăng ký lịch khám): {doctor.GioLamViec}\n");
                }
            }

            //Thêm danh sách dịch vụ
            if (services.Any())
            {
                prompt.AppendLine("\nDưới đây là danh sách dịch vụ khám có sẵn:\n");
                foreach (var service in services)
                {
                    var tenKhoa = service.Khoa != null ? service.Khoa.TenKhoa :
                    (service.KhoaId.HasValue ? (await _departmentRepository.GetByIdAsync(service.KhoaId.Value))?.TenKhoa : "Chưa cập nhật");
                    prompt.AppendLine($"- ID:{service.DichVuId}, Dịch vụ: {service.TenDichVu}, Giá: {service.DonGia:0,0 VNĐ}, " +
                        $"Mô tả: {service.MoTaDichVu}, Khoa (Dịch vụ nằm ở khoa nào): {tenKhoa}\n");
                }
            }
            //Thêm danh sách lịch khám
            if (appointments.Any())
            {
                prompt.AppendLine("\nDanh sách lịch hẹn khám hiện tại:");
                foreach(var appointment in appointments)
                {
                    var doctor = doctors.FirstOrDefault(d => d.BacSiId == appointment.BacSiId);
                    if (doctor != null)
                    {
                        prompt.AppendLine($"- Bác sĩ: {doctor.HoTen}, Ngày khám: {appointment.NgayKham:dd/MM/yyyy}, " +
                                          $"Giờ khám: {appointment.GioKham}, Trạng thái: {appointment.TrangThaiLichKham}.\n");
                    }
                }
            }

            // Hướng dẫn AI cách tư vấn
            prompt.AppendLine("\nHãy tư vấn cho bệnh nhân chọn bác sĩ, dịch vụ khám, lịch khám phù hợp dựa vào danh sách trên:");
            prompt.AppendLine("- Gợi ý bác sĩ phù hợp theo chuyên khoa.");
            prompt.AppendLine("- Đề xuất dịch vụ khám phù hợp với nhu cầu của bệnh nhân.");
            prompt.AppendLine("- Hỗ trợ chọn lịch khám hợp lý theo giờ làm việc của bác sĩ.");
            prompt.AppendLine("- Chú ý: Một bác sĩ giỏi là bác sĩ có bằng cấp là 'Giáo sư Y khoa' hoặc 'Phó Giáo sư Y Khoa'. 'Không' hiển thị id cho bệnh nhân xem, chỉ đưa ra tên tương ứng với id đó.");
            // Hướng dẫn AI cách kiểm tra lịch trống
            prompt.AppendLine("\nHãy tư vấn lịch khám phù hợp dựa trên danh sách trên:");
            prompt.AppendLine("- Kiểm tra lịch hẹn đã có, tránh trùng lặp.");
            prompt.AppendLine("- Đề xuất lịch trống cho bệnh nhân.");
            prompt.AppendLine("- Nếu không có lịch trống trong ngày yêu cầu, hãy đề xuất ngày gần nhất có lịch trống.");
            prompt.AppendLine("- Chú ý: Mỗi ca khám chỉ cho phép có một bệnh nhân, ngày khám phải lớn hơn ngày hiện tại.");
            // Hướng dẫn AI về xác nhận thông tin đặt lịch
            prompt.AppendLine("\nKhi bệnh nhân muốn đặt lịch, hãy xác nhận lại thông tin:");
            prompt.AppendLine($"- Nếu bệnh nhân cung cấp đầy đủ thông tin (họ tên, số điện thoại, email, ngày khám, ca khám, bác sĩ, dịch vụ khám), hãy hỏi: ");
            prompt.AppendLine($"'Bạn có muốn đặt lịch khám với thông tin sau không?' ");
            prompt.AppendLine($"Sau đó, hiển thị thông tin theo định dạng: ");
            prompt.AppendLine($"👉 Họ tên: [Tên bệnh nhân]");
            prompt.AppendLine($"👉 Số điện thoại: [SĐT]");
            prompt.AppendLine($"👉 Email: [Email]");
            prompt.AppendLine($"👉 Ngày khám: [Ngày khám]");
            prompt.AppendLine($"👉 Ca khám: [Giờ khám]");
            prompt.AppendLine($"👉 Bác sĩ: [Tên bác sĩ]");
            prompt.AppendLine($"👉 Dịch vụ: [Tên dịch vụ]");
            prompt.AppendLine($"💬 'Vui lòng xác nhận (Nhập 'Đồng ý' để tiếp tục hoặc 'Chỉnh sửa' nếu cần thay đổi).'");
            prompt.AppendLine("- Nếu thông tin không đầy đủ, yêu cầu bệnh nhân bổ sung.");
            prompt.AppendLine("- Chú ý: Nếu mà trạng thái trong lịch khám là 'Đang xử lý' hoặc 'Đã hủy' thì vẫn có thể đặt lịch được, có 'Đã đặt' hoặc 'Đã hoàn thành' sẽ không đặt được.");
            prompt.AppendLine($"Câu hỏi của người dùng: {userInput}");

            // Gửi yêu cầu đến Gemini
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}";

            var requestData = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[] { new { text = prompt.ToString() } }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Parse JSON để lấy text
            using JsonDocument doc = JsonDocument.Parse(responseString);
            var text = doc.RootElement.GetProperty("candidates")[0]
                   .GetProperty("content")
                   .GetProperty("parts")[0]
                   .GetProperty("text")
                   .GetString();

            // Nếu AI xác nhận thông tin, lưu vào ChatContext
            if (text.Contains("Bạn có muốn đặt lịch khám với thông tin sau không?", StringComparison.OrdinalIgnoreCase))
            {
                context.IsReadyForBooking = true;
                context.PatientName = ExtractValue(text, "Họ tên:");
                context.PatientPhone = ExtractValue(text, "Số điện thoại:");
                context.PatientEmail = ExtractValue(text, "Email:");
                context.AppointmentDate = DateTime.Parse(ExtractValue(text, "Ngày khám:"));
                context.AppointmentTime = ExtractValue(text, "Ca khám:");
                context.DoctorName = ExtractValue(text, "Bác sĩ:");
                context.ServiceName = ExtractValue(text, "Dịch vụ:");

                // Lấy ID từ danh sách đã cung cấp
                context.DoctorId = doctors.FirstOrDefault(d => d.HoTen == context.DoctorName)?.BacSiId ?? Guid.Empty;
                context.ServiceId = services.FirstOrDefault(s => s.TenDichVu == context.ServiceName)?.DichVuId ?? Guid.Empty;
            }

            return text ?? "Không có phản hồi!";
        }

        // Hàm hỗ trợ trích xuất thông tin từ phản hồi của AI
        private string ExtractValue(string text, string label)
        {
            var match = Regex.Match(text, $@"{label} (.*)");
            return match.Success ? match.Groups[1].Value.Trim() : string.Empty;
        }
    }
}
