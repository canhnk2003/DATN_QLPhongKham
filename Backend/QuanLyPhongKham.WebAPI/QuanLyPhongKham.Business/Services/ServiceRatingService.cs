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
            if(res > 0)
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
            if(rating == null)
            {
                throw new ErrorNotFoundException();
            }
            else
            {
                rating.DanhGia = danhGia.DanhGia;
                rating.PhanHoi = danhGia.PhanHoi;
                rating.NgayCapNhat = DateTime.Now;
                var res = await _serviceRatingRepository.UpdateAsync(rating);
                if(res > 0)
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

            // Nhóm theo tên bác sĩ, tính trung bình
            var result = dataWithNames
                .Where(x => !string.IsNullOrEmpty(x.TenBacSi)) // bỏ qua nếu không có tên bác sĩ
                .GroupBy(x => x.TenBacSi)
                .Select(g => new ThongKeDanhGiaModel
                {
                    TenBacSi = g.Key,
                    SoSaoTrungBinh = Math.Round(g.Average(x =>(double) x.DanhGia), 1),
                    SoLuotDanhGia = g.Count()
                });

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
                TenBacSi = d.BacSiId.HasValue ? doctorDict.GetValueOrDefault(d.BacSiId.Value) : null,
                TenBenhNhan = d.BenhNhanId.HasValue ? patientDict.GetValueOrDefault(d.BenhNhanId.Value) : null,
                DanhGia = d.DanhGia,
                PhanHoi = d.PhanHoi
            });

            return result;
        }
    }
}
