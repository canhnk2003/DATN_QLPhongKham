using QuanLyPhongKham.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Data.Interfaces
{
    public interface IPatientRepository : IBaseRepository<BenhNhan>
    {
        /// <summary>
        /// Kiểm tra dữ liệu hợp lệ khi thêm
        /// </summary>
        /// <returns>Mã bệnh nhân mới</returns>
        string GetNextMaBenhNhan();
        /// <summary>
        /// Kiểm tra dữ liệu hợp lệ khi thêm
        /// </summary>
        /// <param name="benhNhan">dữ liệu</param>
        /// <returns>danh sách lỗi</returns>
        Dictionary<string, string>? CheckDataValidate(BenhNhan benhNhan);

        /// <summary>
        /// Kiểm tra dữ liệu hợp lệ khi thêm mới
        /// </summary>
        /// <param name="benhNhan">dữ liệu</param>
        /// <returns>danh sách lỗi</returns>
        Dictionary<string, string>? CheckDataValidateForInsert(BenhNhan benhNhan);

        Task<IEnumerable<BenhNhan>> GetAllByDoctorIdAsync(Guid BacSiId);
        Task<BenhNhan> GetByUserId(string userId);
    }
}
