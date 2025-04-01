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
        // Bổ sung hai thuộc tính mới
        public string? LyDo { get; set; }
        public Guid? DichVuId { get; set; }
    }
}
