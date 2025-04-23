using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Data.Interfaces;
using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Exceptions;
using QuanLyPhongKham.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Business.Services
{
    public class ServiceRatingService : BaseService<DanhGiaDichVu>, IServiceRatingService
    {
        private readonly IServiceRatingRepository _serviceRatingRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;
        public ServiceRatingService(IServiceRatingRepository serviceRatingRepository, IDoctorRepository doctorRepository, IPatientRepository patientRepository) : base(serviceRatingRepository)
        {
            _serviceRatingRepository = serviceRatingRepository;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
        }

        public override async Task<int> AddAsync(DanhGiaDichVu entity)
        {
            entity.DanhGiaId = Guid.NewGuid();
            entity.NgayCapNhat = DateTime.Now;
            entity.NgayTao = DateTime.Now;
            int res = await _serviceRatingRepository.AddAsync(entity);
            if (res > 0)
            {
                return res;
            }
            else
            {
                throw new ErrorCreateException();
            }
        }

        public async Task<int> EditAsync(DanhGiaDichVu danhGia, Guid id)
        {
            var rating = await _serviceRatingRepository.GetByIdAsync(id);
            if (rating == null)
            {
                throw new ErrorNotFoundException();
            }
            else
            {
                if (!string.IsNullOrEmpty(danhGia.BenhNhanId.ToString()))
                    rating.BenhNhanId = danhGia.BenhNhanId;
                if (!string.IsNullOrEmpty(danhGia.BacSiId.ToString()))
                    rating.BacSiId = danhGia.BacSiId;
                if (!string.IsNullOrEmpty(danhGia.LichKhamId.ToString()))
                    rating.LichKhamId = danhGia.LichKhamId;
                if (!string.IsNullOrEmpty(danhGia.DanhGia.ToString()))
                    rating.DanhGia = danhGia.DanhGia;
                if (!string.IsNullOrEmpty(danhGia.PhanHoi))
                    rating.PhanHoi = danhGia.PhanHoi;
                if (!string.IsNullOrEmpty(danhGia.NgayCapNhat.ToString()))
                    rating.NgayCapNhat = DateTime.Now;
                if (!string.IsNullOrEmpty(danhGia.NgayTao.ToString()))
                    rating.NgayTao = danhGia.NgayTao;
                var res = await _serviceRatingRepository.UpdateAsync(rating);
                if (res > 0)
                {
                    return res;
                }
                else
                {
                    throw new ErrorEditException();
                }
            }
        }

        public async Task<IEnumerable<ThongKeDanhGiaModel>> GetAllAverageAsync()
        {
            var ratings = await _serviceRatingRepository.GetAllAsync();
            var doctors = await _doctorRepository.GetAllAsync();

            var doctorDict = doctors.ToDictionary(b => b.BacSiId, b => b.HoTen);

            // Gắn tên bác sĩ cho từng đánh giá
            var dataWithNames = ratings.Select(d => new
            {
                TenBacSi = d.BacSiId.HasValue ? doctorDict.GetValueOrDefault(d.BacSiId.Value) : null,
                DanhGia = d.DanhGia
            });

            // Nhóm theo tên bác sĩ, tính trung bình và số lượt
            var grouped = dataWithNames
                .Where(x => !string.IsNullOrEmpty(x.TenBacSi))
                .GroupBy(x => x.TenBacSi)
                .Select(g => new ThongKeDanhGiaModel
                {
                    TenBacSi = g.Key,
                    SoSaoTrungBinh = Math.Round(g.Average(x => (double)x.DanhGia), 1),
                    SoLuotDanhGia = g.Count()
                })
                .OrderByDescending(x => x.SoSaoTrungBinh)
                .ThenByDescending(x => x.SoLuotDanhGia)
                .ToList();

            int tongSoBacSi = grouped.Count;

            for (int i = 0; i < grouped.Count; i++)
            {
                grouped[i].TongSoBacSi = tongSoBacSi;
                grouped[i].ThuHang = i + 1;
            }

            return grouped;
        }

        public async Task<IEnumerable<DanhGiaDichvuModel>> GetAllRatingByDoctor(Guid doctorId)
        {
            var ratings = await _serviceRatingRepository.GetAllAsync();
            var doctor = await _doctorRepository.GetByIdAsync(doctorId);
            var patients = await _patientRepository.GetAllAsync();

            var patientDict = patients.ToDictionary(b => b.BenhNhanId, b => b.HoTen);

            var result = ratings
                .Where(x => x.BacSiId == doctorId)
                .Select(d => new DanhGiaDichvuModel
                {
                    DanhGiaId = d.DanhGiaId,
                    TenBacSi = doctor?.HoTen,
                    TenBenhNhan = d.BenhNhanId.HasValue ? patientDict.GetValueOrDefault(d.BenhNhanId.Value) : null,
                    DanhGia = d.DanhGia,
                    PhanHoi = d.PhanHoi
                })
                .OrderByDescending(x => x.NgayCapNhat);

            return result;
        }


        public async Task<IEnumerable<DanhGiaDichvuModel>> GetAllWithNameAsync()
        {
            var ratings = await _serviceRatingRepository.GetAllAsync();
            var doctors = await _doctorRepository.GetAllAsync();
            var patients = await _patientRepository.GetAllAsync();

            var doctorDict = doctors.ToDictionary(b => b.BacSiId, b => b.HoTen);
            var patientDict = patients.ToDictionary(b => b.BenhNhanId, b => b.HoTen);

            var result = ratings.Select(d => new DanhGiaDichvuModel
            {
                DanhGiaId = d.DanhGiaId,
                LichKhamId = d.LichKhamId,
                TenBacSi = d.BacSiId.HasValue ? doctorDict.GetValueOrDefault(d.BacSiId.Value) : null,
                TenBenhNhan = d.BenhNhanId.HasValue ? patientDict.GetValueOrDefault(d.BenhNhanId.Value) : null,
                DanhGia = d.DanhGia,
                PhanHoi = d.PhanHoi
            });

            return result;
        }

        //public async Task<DanhGiaDichVu> GetRatingByAppointmentId(Guid id)
        //{
        //    var ratings = await _serviceRatingRepository.GetAllAsync();
        //    var rating = ratings.FirstOrDefault(x=>x.LichKhamId == id);
        //    if(rating == null)
        //    {
        //        throw new ErrorNotFoundException();
        //    }
        //    else
        //    {
        //        return rating;
        //    }
        //}

        public async Task<ThongKeDanhGiaModel?> GetRatingByDoctor(Guid doctorId)
        {
            var ratings = await _serviceRatingRepository.GetAllAsync();
            var doctors = await _doctorRepository.GetAllAsync();

            var doctorDict = doctors.ToDictionary(b => b.BacSiId, b => b.HoTen);

            // Gắn tên bác sĩ cho từng đánh giá
            var dataWithNames = ratings
                .Where(x => x.BacSiId.HasValue && doctorDict.ContainsKey(x.BacSiId.Value))
                .Select(d => new
                {
                    BacSiId = d.BacSiId.Value,
                    TenBacSi = doctorDict[d.BacSiId.Value],
                    DanhGia = d.DanhGia
                });

            // Nhóm theo bác sĩ, tính trung bình
            var grouped = dataWithNames
                .GroupBy(x => new { x.BacSiId, x.TenBacSi })
                .Select(g => new ThongKeDanhGiaModel
                {
                    BacSiId = g.Key.BacSiId,
                    TenBacSi = g.Key.TenBacSi,
                    SoSaoTrungBinh = Math.Round(g.Average(x => (double)x.DanhGia), 1),
                    SoLuotDanhGia = g.Count()
                })
                .OrderByDescending(x => x.SoSaoTrungBinh)
                .ThenByDescending(x => x.SoLuotDanhGia)
                .ToList();

            int tongSoBacSi = grouped.Count;

            for (int i = 0; i < grouped.Count; i++)
            {
                grouped[i].TongSoBacSi = tongSoBacSi;
                grouped[i].ThuHang = i + 1;
            }

            // Lấy đúng bác sĩ được yêu cầu
            return grouped.FirstOrDefault(x => x.BacSiId == doctorId);
        }
    }
}
