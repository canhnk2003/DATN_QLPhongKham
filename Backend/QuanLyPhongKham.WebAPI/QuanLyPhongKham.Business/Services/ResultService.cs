using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Data.Context;
using QuanLyPhongKham.Data.Interfaces;
using QuanLyPhongKham.Data.Repositories;
using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Exceptions;
using QuanLyPhongKham.Models.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Business.Services
{
    public class ResultService : BaseService<KetQuaKham>, IResultService
    {
        private readonly IResultRepository _resultRepository;

        public ResultService(IResultRepository resultRepository) : base(resultRepository)
        {
            _resultRepository = resultRepository;
        }

        // Phương thức lấy kết quả khám của bác sĩ
        public async Task<IEnumerable<KetQuaKham>> GetAllByDoctorIdAsync(Guid bacSiId)
        {
            var results = _resultRepository.GetAllByDoctorId(bacSiId); // Lấy kết quả khám từ repository
            return results;
        }

        public async Task<IEnumerable<KetQuaKham>> GetAllByPatientIdAsync(Guid benhNhanId)
        {
            var results = _resultRepository.GetAllByPatientId(benhNhanId); // Lấy kết quả khám từ repository
            return results;
        }

        public string GetBenhNhanNameByLichKhamId(Guid lichKhamId)
        {
            return _resultRepository.GetBenhNhanNameByLichKhamId(lichKhamId);
        }



        public override async Task<int> AddAsync(KetQuaKham entity)
        {
            // Tạo ID mới cho `KetQuaKham`
            entity.KetQuaKhamId = Guid.NewGuid();
            entity.NgayTao = DateTime.Now;
            entity.NgayCapNhat = DateTime.Now;

            // Kiểm tra dữ liệu hợp lệ khi thêm mới
            var checkData = _resultRepository.CheckDataValidate(entity);

            // Nếu có lỗi dữ liệu, ném ngoại lệ
            if (checkData != null && checkData.Count > 0)
            {
                throw new ErrorValidDataException(checkData);
            }

            // Thực hiện thêm mới entity vào cơ sở dữ liệu
            int res = await _resultRepository.AddAsync(entity);

            // Kiểm tra kết quả thêm mới
            if (res > 0)
            {
                return res; // Trả về kết quả nếu thành công
            }
            else
            {
                throw new ErrorCreateException(); // Ném ngoại lệ nếu tạo thất bại
            }
        }




        public KetQuaKham? GetKetQuaKhamByLichKhamId(Guid lichKhamId)
        {
            var kq = _resultRepository.GetKetQuaKhamByLichKhamId(lichKhamId);
            return kq;

        }


        public override async Task<int> UpdateAsync(KetQuaKham entity)
        {
            var checkData = _resultRepository.CheckDataValidate(entity);

            //Nếu hợp lệ thì cho thêm, không thì lỗi
            if (checkData.Count > 0)
            {
                throw new ErrorValidDataException(checkData);
            }
            else
            {
                int res = await _resultRepository.UpdateAsync(entity);

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
