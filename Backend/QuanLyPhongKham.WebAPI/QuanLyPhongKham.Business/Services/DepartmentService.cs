using QuanLyPhongKham.Business.Interfaces;
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
    public class DepartmentService : BaseService<Khoa>, IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(IBaseRepository<Khoa> repository, IDepartmentRepository departmentRepository) : base(repository)
        {
            _departmentRepository = departmentRepository;
        }
        public override async Task<int> AddAsync(Khoa entity)
        {
            var error = CheckDataValidate(entity);
            if(error.Count > 0)
            {
                throw new ErrorValidDataException(error);
            }
            else
            {
                entity.KhoaId = Guid.NewGuid();
                entity.MaKhoa = await GetNextMaKhoa();
                entity.NgayTao = DateTime.Now;
                entity.NgayCapNhat = DateTime.Now;
                int res = await _departmentRepository.AddAsync(entity);
                if(res > 0)
                {
                    return res;
                }
                else
                {
                    throw new ErrorCreateException();
                }
            }
        }

        public override async Task<int> UpdateAsync(Khoa entity)
        {
            var error = CheckDataValidate(entity);
            if (error.Count > 0)
            {
                throw new ErrorValidDataException(error);
            }
            else
            {
                var khoaExist = await _departmentRepository.GetByIdAsync(entity.KhoaId);
                if(!string.IsNullOrEmpty(entity.TenKhoa))
                    khoaExist.TenKhoa = entity.TenKhoa;
                khoaExist.NgayCapNhat = DateTime.Now;
                int res = await _departmentRepository.UpdateAsync(khoaExist);
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
        private Dictionary<string, string>? CheckDataValidate(Khoa khoa)
        {
            var errorData = new Dictionary<string, string>();

            //Kiểm tra ngày khám không được nhỏ hơn ngày hiện tại và không được trống
            if (string.IsNullOrEmpty(khoa.TenKhoa.ToString()))
            {
                errorData.Add("TenKhoa", "Tên khoa không được để trống!");
            }
            
            return errorData;
        }

        private async Task<string> GetNextMaKhoa()
        {
            // Lấy danh sách mã khoa
            var dsKhoa = await _departmentRepository.GetAllAsync();
            var maxMaKhoa = dsKhoa.
                AsEnumerable().
                Select(k => new
                {
                    So = int.Parse(k.MaKhoa.Substring(2))
                })
                .OrderByDescending(k => k.So)
                .FirstOrDefault();

            // Nếu không có bệnh nhân nào trong hệ thống, mã bắt đầu từ "K001"
            if (maxMaKhoa == null)
            {
                return "K001";
            }

            // Lấy phần số lớn nhất và cộng thêm 1
            int nextSo = maxMaKhoa.So + 1;

            // Kết hợp phần chữ "K" và phần số (cộng thêm 1), định dạng phần số với 3 chữ số
            return $"K{nextSo:D3}";
        }
    }
}
