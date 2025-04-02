using Microsoft.Extensions.Options;
using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Data.Interfaces;
using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public GeminiService(IOptions<GoogleAISettings> settings, IServiceRepository serviceRepository, IDoctorRepository doctorRepository, IDepartmentRepository departmentRepository)
        {
            _httpClient = new HttpClient();
            _apiKey = settings.Value.ApiKey;
            _serviceRepository = serviceRepository;
            _doctorRepository = doctorRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<string> GetResponseFromGemini(string userInput, ChatContext context)
        {
            // Lấy danh sách bác sĩ từ database
            var doctors = await _doctorRepository.GetAllAsync();

            if (!doctors.Any())
            {
                return "Hiện tại chưa có dữ liệu về bác sĩ!";
            }

            // Tạo nội dung gửi cho Gemini
            var prompt = new StringBuilder();
            prompt.AppendLine("Bạn là một trợ lý AI giúp bệnh nhân tìm kiếm bác sĩ.");
            prompt.AppendLine("Dưới đây là danh sách bác sĩ trong hệ thống:\n");

            foreach (var doctor in doctors)
            {
                var tenKhoa = doctor.Khoa != null ? doctor.Khoa.TenKhoa :
                    (doctor.KhoaId.HasValue ? (await _departmentRepository.GetByIdAsync(doctor.KhoaId.Value))?.TenKhoa : "Chưa cập nhật");

                prompt.AppendLine($"- Họ tên: {doctor.HoTen}, Bằng cấp: {doctor.TenBangCap}, Chuyên khoa: {tenKhoa}" +
                    $"Số điện thoại: {doctor.SoDienThoai}, Địa chỉ: {doctor.DiaChi}, Kinh nghiệm: {doctor.SoNamKinhNghiem} năm, " +
                    $"Giờ làm việc (là giờ có thể đăng ký lịch khám): {doctor.GioLamViec}\n");
            }

            prompt.AppendLine("Dựa trên danh sách trên, hãy trả lời câu hỏi của người dùng một cách chính xác. Gợi ý bác sĩ giỏi là bác sĩ có bằng cấp là Giáo sư Y khoa hoặc Bác sĩ Chuyên khoa 1.");
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

            return responseString;
        }



        // 🎯 Hàm riêng để xử lý tìm kiếm bác sĩ theo câu hỏi người dùng
        //private string FindDoctorResponse(string userInput, IEnumerable<BacSi> doctors, ChatContext context)
        //{
        //    if (userInput.ToLower().Contains("bác sĩ"))
        //    {
        //        var match = Regex.Match(userInput, @"khoa\s+([\p{L}\s]+)", RegexOptions.IgnoreCase);
        //        if (match.Success)
        //        {
        //            string specialization = match.Groups[1].Value;
        //            var filteredDoctors = doctors.Where(b => b.Khoa != null && b.Khoa.TenKhoa.ToLower().Contains(specialization.ToLower())).ToList();

        //            if (filteredDoctors.Any())
        //            {
        //                context.LastTopic = "doctor";
        //                context.LastSpecialization = specialization;
        //                return $"Dưới đây là các bác sĩ thuộc chuyên khoa {specialization}: " +
        //                       string.Join("\n", filteredDoctors.Select(d => $"{d.HoTen} - {d.TenBangCap}"));
        //            }
        //        }
        //        else
        //        {
        //            context.LastTopic = "doctor";
        //            return "Dưới đây là danh sách tất cả các bác sĩ: " +
        //                   string.Join("\n", doctors.Select(d => $"{d.HoTen} - {d.TenBangCap}"));
        //        }
        //    }

        //    // Nếu người dùng hỏi thêm về bác sĩ mà không nói rõ tên
        //    if (context.LastTopic == "doctor" && !doctors.Any(d => userInput.ToLower().Contains(d.HoTen.ToLower())))
        //    {
        //        if (!string.IsNullOrEmpty(context.LastSpecialization))
        //        {
        //            var topDoctors = doctors.Where(b => b.Khoa != null &&
        //                                                b.Khoa.TenKhoa.ToLower().Contains(context.LastSpecialization.ToLower()) &&
        //                                                (b.TenBangCap == "Giáo sư Y khoa" || b.TenBangCap == "Bác sĩ Chuyên khoa 1"))
        //                                    .ToList();
        //            if (topDoctors.Any())
        //            {
        //                return $"Bác sĩ giỏi nhất trong chuyên khoa {context.LastSpecialization} là: " +
        //                       string.Join("\n", topDoctors.Select(d => $"{d.HoTen} - {d.TenBangCap}"));
        //            }
        //        }
        //        return "Bạn có thể hỏi thêm về một bác sĩ cụ thể!";
        //    }

        //    // Nếu người dùng hỏi chi tiết về một bác sĩ cụ thể
        //    var matchedDoctors = doctors.Where(d => userInput.ToLower().Contains(d.HoTen.ToLower())).ToList();
        //    if (matchedDoctors.Any())
        //    {
        //        return $"Thông tin bác sĩ {matchedDoctors.First().HoTen}: " +
        //               string.Join("\n", matchedDoctors.Select(b => $"{b.HoTen} - {b.TenBangCap} - {b.Khoa.TenKhoa}"));
        //    }

        //    return string.Empty; // Trả về chuỗi rỗng nếu không có kết quả nào
        //}
    }
}
