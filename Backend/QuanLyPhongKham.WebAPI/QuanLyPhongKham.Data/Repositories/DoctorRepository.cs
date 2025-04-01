using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data.Context;
using QuanLyPhongKham.Data.Interfaces;
using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Data.Repositories
{
    public class DoctorRepository : BaseRepository<BacSi>, IDoctorRepository
    {
        public DoctorRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Kiểm tra dữ liệu hợp lệ khi sửa
        /// </summary>
        /// <param name="bacSi"></param>
        /// <returns>Danh sách lỗi</returns>

        public Dictionary<string, string>? CheckDataValidate(BacSi bacSi)
        {
            var errorData = new Dictionary<string, string>();

            //1. Kiểm tra 1 số thông tin không được trống
            //1.1. Mã bác sĩ không được để trống
            if (string.IsNullOrEmpty(bacSi.MaBacSi))
            {
                errorData.Add("MaBacSi", ResourceVN.Error_MaBacSiNotEmpty);
            }

            //1.2. Họ tên không được để trống
            if (string.IsNullOrEmpty(bacSi.HoTen))
            {
                errorData.Add("HoTen", ResourceVN.Error_HoTenNotEmpty);
            }

            //1.3. Email không được để trống
            if (string.IsNullOrEmpty(bacSi.Email))
            {
                errorData.Add("Email", ResourceVN.Error_EmailNotEmpty);
            }
            else
            {
                //Kiểm tra Email đúng định dạng
                if (CheckEmailValid(bacSi.Email) == false)
                {
                    errorData.Add("Email", ResourceVN.Error_ValidEmail);
                }
            }

            //2. Thực hiện validate dữ liệu
            //2.1. Họ tên không được có số
            if (bacSi.HoTen.Any(char.IsDigit))
            {
                errorData.Add("FullName", ResourceVN.Error_HoTenNotNumber);
            }

            //2.2. Số điện thoại không được có chữ
            if (!string.IsNullOrEmpty(bacSi.SoDienThoai) && bacSi.SoDienThoai.Any(char.IsLetter))
            {
                errorData.Add("PhoneNumber", ResourceVN.Error_PhoneNumberNotLetter);
            }
            return errorData;
        }

        /// <summary>
        /// Kiểm tra định dạng email
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>
        /// true - không trùng
        /// false - trùng
        /// </returns>
        private bool CheckEmailValid(string email)
        {
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            if (Regex.IsMatch(email, pattern))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Kiểm tra dữ liệu hợp lệ khi thêm
        /// </summary>
        /// <param name="bacSi">dữ liệu</param>
        /// <returns>danh sách lỗi</returns>

        public Dictionary<string, string>? CheckDataValidateForInsert(BacSi bacSi)
        {
            var errorData = CheckDataValidate(bacSi);

            bool ma = _dbSet.Any(b => b.MaBacSi == bacSi.MaBacSi);

            if (ma)
            {
                errorData.Add("DoctorCode", ResourceVN.Error_MaBacSiDuplicated);
            }
            return errorData;
        }

        public string GetNextMaBacSi()
        {
            //Lấy danh sách mã bác sĩ
            var maxMaBS = _context.BacSis
                .AsEnumerable() //Chuyển sang client side
                .Select(bs => new
                {
                    MaBS = bs.MaBacSi,
                    So = int.Parse(bs.MaBacSi.Substring(2)) // Tách phần số, bỏ qua 2 ký tự đầu tiên "BS"
                })
                .OrderByDescending(bs => bs.So) // Sắp xếp theo số, giảm dần
                .FirstOrDefault();

            // Nếu không có bác sĩ nào trong hệ thống, mã bắt đầu từ "BS001"
            if (maxMaBS == null)
            {
                return "BS001";
            }

            // Lấy phần số lớn nhất và cộng thêm 1
            int nextSo = maxMaBS.So + 1;

            // Kết hợp phần chữ "BS" và phần số (cộng thêm 1), định dạng phần số với 3 chữ số
            return $"BS{nextSo:D3}";
        }

        public async Task<BacSi> GetByUserId(string userId)
        {
            var bn = await _context.BacSis.Where(bn => bn.UserId == userId).FirstOrDefaultAsync();
            return bn;
        }

        public async Task<IEnumerable<BacSi>> GetBacSisByKhoaId(Guid id)
        {
            return await _context.BacSis.Where(b => b.KhoaId == id).ToListAsync();
        }
    }
}
