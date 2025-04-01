using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Models.Models
{
    public class BacSiModel
    {
        public Guid BacSiId { get; set; }
        public Guid? KhoaId { get; set; }
        public string MaBacSi { get; set; }
        public string HoTen { get; set; }
        public string? HinhAnh { get; set; }
        public string? SoDienThoai { get; set; }
        public string? Email { get; set; }
        public string? DiaChi { get; set; }
        public TrinhDo? BangCap { get; set; }
        public string? TenBangCap
        {
            get
            {
                switch (BangCap)
                {
                    case (TrinhDo.GiaoSuYKhoa):
                        return "Giáo sư Y khoa";
                    case (TrinhDo.BacSiChuyenKhoa1):
                        return "Bác sĩ Chuyên khoa 1";
                    case (TrinhDo.BacSiChuyenKhoa2):
                        return "Bác sĩ Chuyên khoa 2";
                    case (TrinhDo.BacSiDaKhoa):
                        return "Bác sĩ Đa khoa";
                    case (TrinhDo.ThacSiYKhoa):
                        return "Thạc sĩ Y khoa";
                    case (TrinhDo.PhoGiaoSuYKhoa):
                        return "Phó Giáo sư Y khoa";
                    case (TrinhDo.TienSiYKhoa):
                        return "Tiến sĩ Y khoa";
                    default:
                        return string.Empty;
                }
            }
        }
        public int? SoNamKinhNghiem { get; set; }
        public string? GioLamViec { get; set; }
    }
}
