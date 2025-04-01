using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Data.Interfaces;
using QuanLyPhongKham.Data.Repositories;
using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Business.Services
{
    public class PatientService : BaseService<BenhNhan>, IPatientService
    {
        private readonly IPatientRepository _patientRepository;

        public PatientService(IPatientRepository patientRepository) : base(patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public override async Task<int> AddAsync(BenhNhan entity)
        {
            string maBNNext = _patientRepository.GetNextMaBenhNhan();
            entity.BenhNhanId = Guid.NewGuid();
            entity.MaBenhNhan = maBNNext;
            //Kiểm tra dữ liệu hợp lệ
            var checkData = _patientRepository.CheckDataValidateForInsert(entity);

            //Nếu hợp lệ thì cho thêm, không thì lỗi
            if (checkData.Count > 0)
            {
                throw new ErrorValidDataException(checkData);
            }
            else
            {
                int res = await _patientRepository.AddAsync(entity);
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

        public override async Task<int> UpdateAsync(BenhNhan entity)
        {
            var checkData = _patientRepository.CheckDataValidate(entity);

            //Nếu hợp lệ thì cho thêm, không thì lỗi
            if (checkData.Count > 0)
            {
                throw new ErrorValidDataException(checkData);
            }
            else
            {
                int res = await _patientRepository.UpdateAsync(entity);

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

        public async Task<IEnumerable<BenhNhan>> GetAllByDoctorIdAsync(Guid BacSiId)
        {
            return await _patientRepository.GetAllByDoctorIdAsync(BacSiId);
        }

        public async Task<BenhNhan> GetByUserId(string userId)
        {
            var bn = await _patientRepository.GetByUserId(userId);
            if (bn == null)
            {
                throw new ErrorNotFoundException();
            }
            return bn;
        }
    }
}
