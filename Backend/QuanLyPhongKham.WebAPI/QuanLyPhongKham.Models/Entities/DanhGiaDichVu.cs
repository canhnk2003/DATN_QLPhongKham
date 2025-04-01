using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Models.Entities
{
    public class DanhGiaDichVu
    {
        public Guid DanhGiaId { get; set; }
        public Guid? BenhNhanId { get; set; }
        public Guid? BacSiId { get; set; }
        public int? DanhGia { get; set; } = 0;
        public string? PhanHoi { get; set; }
        public DateTime? NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; } = DateTime.Now;
        public BenhNhan? BenhNhan { get; set; }
        public BacSi? BacSi { get; set; }
    }
}
