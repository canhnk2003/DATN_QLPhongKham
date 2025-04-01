using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Models.Entities
{
    public class LichKham
    {
        public Guid LichKhamId { get; set; }
        public Guid BenhNhanId { get; set; }
        public Guid BacSiId { get; set; }
        public DateTime? NgayKham { get; set; }
        public string GioKham { get; set; }
        public string TrangThaiLichKham { get; set; }
        public DateTime? NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; } = DateTime.Now;

        // Bổ sung hai thuộc tính mới
        public string? LyDo { get; set; }
        public Guid? DichVuId { get; set; }

        public BenhNhan BenhNhan { get; set; }
        public BacSi BacSi { get; set; }
        public virtual KetQuaKham? KetQuaKham { get; set; }

    }
}
