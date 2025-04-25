using QuanLyPhongKham.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Models.Models
{
    public class DanhGiaDichvuModel
    {
        public Guid DanhGiaId { get; set; }
        public Guid? BenhNhanId { get; set; }
        public Guid? BacSiId { get; set; }
        public Guid? LichKhamId { get; set; }
        public string? TenBacSi {  get; set; }
        public string? TenBenhNhan {  get; set; }
        public string? TenDichVu { get; set; }
        public int? DanhGia { get; set; } = 0;
        public string? PhanHoi { get; set; }
        public DateTime? NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; } = DateTime.Now;
    }
}
