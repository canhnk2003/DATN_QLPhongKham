using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Data.Interfaces;
using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Exceptions;
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
        public ServiceRatingService(IServiceRatingRepository serviceRatingRepository) : base(serviceRatingRepository)
        {
            _serviceRatingRepository = serviceRatingRepository;
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
    }
}
