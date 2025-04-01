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
    public class PatientRepository : BaseRepository<BenhNhan>, IPatientRepository
    {
        public PatientRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Kiểm tra dữ liệu hợp lệ khi sửa
        /// </summary>
        /// <param name="benhNhan"></param>
        /// <returns>Danh sách lỗi</returns>
        public Dictionary<string, string>? CheckDataValidate(BenhNhan benhNhan)
        {
            var errorData = new Dictionary<string, string>();

            //1. Kiểm tra 1 số thông tin không được trống
            //1.1. Mã bênh nhân không được để trống
            if (string.IsNullOrEmpty(benhNhan.MaBenhNhan))
            {
                errorData.Add("MaBenhNhan", ResourceVN.Error_MaBenhNhanNotEmpty);
            }

            //1.2. Họ tên bệnh không được để trống
            if (string.IsNullOrEmpty(benhNhan.HoTen))
            {
                errorData.Add("HoTen", ResourceVN.Error_HoTenNotEmpty);
            }

            //1.3. Email không được để trống
            if (string.IsNullOrEmpty(benhNhan.Email))
            {
                errorData.Add("Email", ResourceVN.Error_EmailNotEmpty);
            }
            else
            {
                //Kiểm tra Email đúng định dạng
                if (CheckEmailValid(benhNhan.Email) == false)
                {
                    errorData.Add("Email", ResourceVN.Error_ValidEmail);
                }
            }
            //2. Thực hiện validate dữ liệu
            //2.1. Họ tên không được có số
            //if (benhNhan.HoTen.Any(char.IsDigit))
            //{
            //    errorData.Add("FullName", ResourceVN.Error_HoTenNotNumber);
            //}
            //2.2. Số điện thoại không được có chữ
            if (!string.IsNullOrEmpty(benhNhan.SoDienThoai) && benhNhan.SoDienThoai.Any(char.IsLetter))
            {
                errorData.Add("SoDienThoai", ResourceVN.Error_PhoneNumberNotLetter);
            }
            //2.3. Ngày sinh không được lớn hơn ngày hiện tại
            if (benhNhan.NgaySinh.HasValue && benhNhan.NgaySinh > DateTime.Now)
            {
                errorData.Add("NgaySinh", ResourceVN.Error_BOfDateNotGreatNow);
            }
            return errorData;
        }

        /// <summary>
        /// Kiểm tra dữ liệu hợp lệ khi thêm
        /// </summary>
        /// <param name="benhNhan">dữ liệu</param>
        /// <returns>danh sách lỗi</returns>
        public Dictionary<string, string>? CheckDataValidateForInsert(BenhNhan benhNhan)
        {
            var errorData = CheckDataValidate(benhNhan);

            bool ma = _dbSet.Any(b => b.MaBenhNhan == benhNhan.MaBenhNhan);

            if (ma)
            {
                errorData.Add("MaBenhNhan", ResourceVN.Error_MaBenhNhanDuplicated);
            }
            return errorData;
        }


        public string GetNextMaBenhNhan()
        {
            // Lấy danh sách mã bệnh nhân
            var maxMaBN = _context.BenhNhans
                .AsEnumerable() // Chuyển sang client side
                .Select(bn => new
                {
                    MaBN = bn.MaBenhNhan,
                    So = int.Parse(bn.MaBenhNhan.Substring(2)) // Tách phần số, bỏ qua 2 ký tự đầu tiên "BN"
                })
                .OrderByDescending(bn => bn.So) // Sắp xếp theo số, giảm dần
                .FirstOrDefault();

            // Nếu không có bệnh nhân nào trong hệ thống, mã bắt đầu từ "BN001"
            if (maxMaBN == null)
            {
                return "BN001";
            }

            // Lấy phần số lớn nhất và cộng thêm 1
            int nextSo = maxMaBN.So + 1;

            // Kết hợp phần chữ "BN" và phần số (cộng thêm 1), định dạng phần số với 3 chữ số
            return $"BN{nextSo:D3}";
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

        public async Task<IEnumerable<BenhNhan>> GetAllByDoctorIdAsync(Guid BacSiId)
        {
            //var benhNhans = await _context.BenhNhans
            //.Where(bn => bn.LichKhams.Any(lk => lk.BacSiId == BacSiId))
            //.Include(bn => bn.LichKhams) // Bao gồm các lịch khám của bệnh nhân
            //.ToListAsync();

            var benhNhans = await _context.LichKhams
                                    .Where(lk => lk.BacSiId == BacSiId)
                                    .Select(lk => lk.BenhNhan)
                                    .Distinct() // Loại bỏ trùng lặp
                                    .ToListAsync();
            return benhNhans;
        }

        public async Task<BenhNhan> GetByUserId(string userId)
        {
            var bn = await _context.BenhNhans.Where(bn => bn.UserId == userId).FirstOrDefaultAsync();
            return bn;
        }
    }
}
