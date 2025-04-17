using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Models.Models
{
    public class KetQuaKhamModel
    {
        public Guid KetQuaKhamId { get; set; }
        public string ChanDoan { get; set; }
        public string? ChiDinhThuoc { get; set; }
        public string? GhiChu { get; set; }
        public DateTime? NgayKham { get; set; }
        public string GioKham { get; set; }
        public BacSiModel BacSi { get; set; }
        public BenhNhanModel BenhNhan { get; set; }
        public DichVuModel DichVu { get; set; }
    }
}
