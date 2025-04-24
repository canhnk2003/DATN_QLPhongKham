using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Data.Interfaces;
using QuanLyPhongKham.Data.Repositories;
using QuanLyPhongKham.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Business.Services
{
    public class HomeService : IHomeService
    {
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IPatientRepository _patientRepo;
        private readonly IServiceRatingRepository _ratingRepo;
        private readonly IDoctorRepository _doctorRepo;
        private readonly IDepartmentRepository _departmentRepo;
        public HomeService(IAppointmentRepository appointmentRepo, IPatientRepository patientRepo, IDoctorRepository doctorRepo, IServiceRatingRepository serviceRatingRepository, IDepartmentRepository departmentRepository)
        {
            _appointmentRepo = appointmentRepo;
            _patientRepo = patientRepo;
            _doctorRepo = doctorRepo;
            _ratingRepo = serviceRatingRepository;
            _departmentRepo = departmentRepository;
        }

        public async Task<IEnumerable<ThongKeDanhGiaModel>> GetAllDoctorInfor()
        {
            var ratings = await _ratingRepo.GetAllAsync();
            var doctors = await _doctorRepo.GetAllAsync();
            var khoas = await _departmentRepo.GetAllAsync();

            // Tạo dictionary tra cứu khoa
            var khoaDict = khoas.ToDictionary(k => k.KhoaId, k => k.TenKhoa);

            // Group đánh giá theo BacSiId
            var groupedRatings = ratings
                .Where(x => x.BacSiId != null)
                .GroupBy(x => x.BacSiId.Value)
                .ToDictionary(g => g.Key, g => new
                {
                    SoSaoTrungBinh = Math.Round(g.Average(r => (double)r.DanhGia), 1),
                    SoLuotDanhGia = g.Count()
                });

            // Gộp dữ liệu bác sĩ + đánh giá
            var result = doctors
                .Where(b => groupedRatings.ContainsKey(b.BacSiId)) // Chỉ lấy bác sĩ có đánh giá
                .Select(b => new ThongKeDanhGiaModel
                {
                    BacSiId = b.BacSiId,
                    TenBacSi = b.HoTen,
                    SoSaoTrungBinh = groupedRatings[b.BacSiId].SoSaoTrungBinh,
                    SoLuotDanhGia = groupedRatings[b.BacSiId].SoLuotDanhGia,
                    BangCap = !string.IsNullOrEmpty(b.TenBangCap) ? b.TenBangCap : "",
                    MaBacSi = b.MaBacSi,
                    SoNamKinhNghiem = b.SoNamKinhNghiem ?? 0,
                    TenKhoa = b.KhoaId.HasValue ? khoaDict.GetValueOrDefault(b.KhoaId.Value, "") : ""
                })
                .OrderByDescending(x => x.SoSaoTrungBinh)
                .ThenByDescending(x => x.SoLuotDanhGia)
                .ToList();

            int tongSoBacSi = result.Count;

            for (int i = 0; i < result.Count; i++)
            {
                result[i].TongSoBacSi = tongSoBacSi;
                result[i].ThuHang = i + 1;
            }

            return result;
        }


        public async Task<ClinicInfor> GetClinicInfor()
        {
            //Lấy ra danh sách lịch khám
            var appointments = await _appointmentRepo.GetAllAsync();

            //Lấy ra danh sách bệnh nhân
            var patients = await _patientRepo.GetAllAsync();

            //Tổng lịch khám
            int sumApp = appointments.Count();

            //Tổng lịch khám thành công
            int sumAppSuc = appointments.Where(x => x.TrangThaiLichKham == "Hoàn thành" 
                            || x.TrangThaiLichKham == "Đã hoàn thành").Count();
            
            //Tổng lịch khám hủy
            int sumAppCan = appointments.Where(x => x.TrangThaiLichKham == "Đã hủy").Count();

            //Tổng số bệnh nhân
            int sumPat = patients.Count();

            ClinicInfor clinic = new ClinicInfor();
            clinic.SumAppointment = sumApp;
            clinic.SumAppointmentSuccess = sumAppSuc;
            clinic.SumAppointmentCancel = sumAppCan;
            clinic.SumPatient = sumPat;

            return clinic;
        }
    }
}
