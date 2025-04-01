using QuanLyPhongKham.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Models.Models
{
    public class BenhNhanModel
    {
        public Guid BenhNhanId { get; set; }
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public string? HinhAnh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public GioiTinh? LoaiGioiTinh { get; set; }
        public string? GioiTinh
        {
            get
            {
                switch (LoaiGioiTinh)
                {
                    case Enums.GioiTinh.Nam:
                        return "Nam";
                    case Enums.GioiTinh.Nu:
                        return "Nữ";
                    default:
                        return "Khác";
                }
            }
        }
        public string? SoDienThoai { get; set; }
        public string? Email { get; set; }
        public string? DiaChi { get; set; }
        public string? TienSuBenhLy { get; set; }
    }
}
