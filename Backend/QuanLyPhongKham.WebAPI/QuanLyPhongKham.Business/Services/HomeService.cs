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
        private readonly IServiceRepository _serviceRepo;
        public HomeService(IAppointmentRepository appointmentRepo, IPatientRepository patientRepo, IDoctorRepository doctorRepo, IServiceRatingRepository serviceRatingRepository, IDepartmentRepository departmentRepository, IServiceRepository serviceRepo)
        {
            _appointmentRepo = appointmentRepo;
            _patientRepo = patientRepo;
            _doctorRepo = doctorRepo;
            _ratingRepo = serviceRatingRepository;
            _departmentRepo = departmentRepository;
            _serviceRepo = serviceRepo;
        }
        public async Task<ClinicInfor> GetClinicInfor(Guid? doctorId = null)
        {
            var appointments = await _appointmentRepo.GetAllAsync();
            var patients = await _patientRepo.GetAllAsync();

            int sumApp = 0;
            int sumAppSuc = 0;
            int sumAppCan = 0;
            int sumPat = 0;

            // Nếu có truyền doctorId thì lọc lịch khám theo bác sĩ đó
            if (doctorId.HasValue)
            {
                appointments = appointments
                    .Where(a => a.BacSiId == doctorId.Value)
                    .ToList();

                // Lấy danh sách bệnh nhân duy nhất của bác sĩ đó
                var patientIdsByDoctor = appointments
                                        .Select(a => a.BenhNhanId)
                                        .Distinct()
                                        .ToList();

                sumPat = patients
                    .Where(p => patientIdsByDoctor.Contains(p.BenhNhanId))
                    .Count();
            }
            else
            {
                // Nếu không truyền doctorId thì tổng số bệnh nhân là tất cả
                sumPat = patients.Count();
            }

            // Thống kê số lịch khám theo trạng thái
            foreach (var a in appointments)
            {
                sumApp++;
                if (a.TrangThaiLichKham == "Hoàn thành" || a.TrangThaiLichKham == "Đã hoàn thành")
                    sumAppSuc++;
                else if (a.TrangThaiLichKham == "Đã hủy")
                    sumAppCan++;
            }

            ClinicInfor clinic = new ClinicInfor
            {
                SumAppointment = sumApp,
                SumAppointmentSuccess = sumAppSuc,
                SumAppointmentCancel = sumAppCan,
                SumPatient = sumPat
            };

            return clinic;
        }


        public async Task<IEnumerable<ThongKeDichVuTheoNamModel>> ThongKeDichVuPhoBienTheoNamAsync()
        {
            var appointments = await _appointmentRepo.GetAllAsync();
            var services = await _serviceRepo.GetAllAsync(); // lấy tên dịch vụ từ bảng dịch vụ
            var result = appointments
                .Where(a => a.NgayKham.HasValue && a.DichVuId != null)
                .GroupBy(a => a.NgayKham.Value.Year)
                .Select(groupByYear =>
                {
                    var total = groupByYear.Count();

                    var topDichVu = groupByYear
                        .GroupBy(a => a.DichVuId)
                        .Select(g => new
                        {
                            DichVuId = g.Key,
                            SoLuong = g.Count()
                        })
                        .OrderByDescending(x => x.SoLuong)
                        .ToList();

                    var top5 = topDichVu.Take(5).ToList();
                    var otherTotal = total - top5.Sum(x => x.SoLuong);

                    var top5WithNames = top5.Select(x => new DichVuPhanTramModel
                    {
                        TenDichVu = services.FirstOrDefault(s => s.DichVuId == x.DichVuId)?.TenDichVu ?? "Không có",
                        PhanTram = Math.Round((double)x.SoLuong * 100 / total, 2)
                    }).ToList();

                    if (otherTotal > 0)
                    {
                        top5WithNames.Add(new DichVuPhanTramModel
                        {
                            TenDichVu = "Khác",
                            PhanTram = Math.Round((double)otherTotal * 100 / total, 2)
                        });
                    }

                    return new ThongKeDichVuTheoNamModel
                    {
                        Nam = groupByYear.Key,
                        TopDichVu = top5WithNames
                    };
                })
                .OrderByDescending(x => x.Nam)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<ThongKeLichKhamTheoNamModel>> ThongKeLichKhamTheoNamAsync(Guid? doctorId = null)
        {
            var appointments = await _appointmentRepo.GetAllAsync();

            var filteredAppointments = appointments
                .Where(a => a.NgayKham.HasValue);

            if (doctorId.HasValue)
            {
                filteredAppointments = filteredAppointments
                    .Where(a => a.BacSiId == doctorId.Value);
            }

            var statisticals = filteredAppointments
                .GroupBy(a => a.NgayKham.Value.Year)
                .Select(g => new ThongKeLichKhamTheoNamModel
                {
                    Nam = g.Key,
                    SoLuongTheoThang = Enumerable.Range(1, 12)
                        .Select(thang => g.Count(a => a.NgayKham.Value.Month == thang))
                        .ToArray()
                })
                .OrderByDescending(x => x.Nam)
                .ToList();

            return statisticals;
        }

        public async Task<IEnumerable<ThongKeLichKhamTheoTrangThaiModel>> ThongKeLichKhamTheoTrangThaiAsync(Guid? doctorId = null)
        {
            var appointments = await _appointmentRepo.GetAllAsync();

            if (doctorId.HasValue)
            {
                appointments = appointments.Where(a => a.BacSiId == doctorId.Value);
            }

            var result = appointments
                .Where(a => !string.IsNullOrEmpty(a.TrangThaiLichKham))
                .GroupBy(a => a.TrangThaiLichKham)
                .Select(g => new ThongKeLichKhamTheoTrangThaiModel
                {
                    TrangThai = g.Key,
                    SoLuong = g.Count()
                })
                .OrderByDescending(x => x.SoLuong)
                .ToList();

            return result;
        }
    }
}
