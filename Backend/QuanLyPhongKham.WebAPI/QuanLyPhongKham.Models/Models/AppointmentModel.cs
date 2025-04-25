using QuanLyPhongKham.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Models.Models
{
    public class AppointmentModel
    {
        public Guid LichKhamId { get; set; }
        public Guid BenhNhanId { get; set; }
        public Guid BacSiId { get; set; }
        public DateTime? NgayKham { get; set; }
        public string GioKham { get; set; }
        public string TrangThaiLichKham { get; set; }
        public BenhNhan BenhNhan { get; set; }
        public string? LyDo { get; set; }
        public Guid? DichVuId { get; set; }
        public string? MaBenhNhan { get; set; }
        public string? TenBenhNhan { get; set; }
        public string? TenBacSi {  get; set; }
        public string? TenDichVu {  get; set; }
        public DateTime? NgayCapNhat { get; set; }
    }
}
