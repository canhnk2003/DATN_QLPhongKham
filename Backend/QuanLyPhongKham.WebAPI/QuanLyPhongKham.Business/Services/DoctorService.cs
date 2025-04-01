using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Data.Interfaces;
using QuanLyPhongKham.Data.Repositories;
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
    public class DoctorService : BaseService<BacSi>, IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorService(IDoctorRepository doctorRepository) : base(doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public override async Task<int> AddAsync(BacSi entity)
        {
            string maBSNext = _doctorRepository.GetNextMaBacSi();
            entity.BacSiId = Guid.NewGuid();
            entity.MaBacSi = maBSNext;
            //Kiểm tra dữ liệu hợp lệ
            var checkData = _doctorRepository.CheckDataValidateForInsert(entity);

            //Nếu hợp lệ thì cho thêm, không thì lỗi
            if (checkData.Count > 0)
            {
                throw new ErrorValidDataException(checkData);
            }
            else
            {
                int res = await _doctorRepository.AddAsync(entity);
                if (res > 0)
                {
                    return res;
                }
                else
                {
                    throw new ErrorCreateException();
                }
            }
        }

        public async Task<BacSi> GetByUserId(string userId)
        {
            var bs = await _doctorRepository.GetByUserId(userId);
            if (bs == null)
            {
                throw new ErrorNotFoundException();
            }
            return bs;

        }

        public async Task<IEnumerable<BacSi>> GetBacSisByKhoaId(Guid id)
        {
            return await _doctorRepository.GetBacSisByKhoaId(id);
        }

        public override async Task<int> UpdateAsync(BacSi entity)
        {
            var checkData = _doctorRepository.CheckDataValidate(entity);

            //Nếu hợp lệ thì cho thêm, không thì lỗi
            if (checkData.Count > 0)
            {
                throw new ErrorValidDataException(checkData);
            }
            else
            {
                int res = await _doctorRepository.UpdateAsync(entity);

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
    }
    
}
