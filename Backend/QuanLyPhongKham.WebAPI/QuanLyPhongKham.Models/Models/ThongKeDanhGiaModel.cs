using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Models.Models
{
    public class ThongKeDanhGiaModel
    {
        public Guid BacSiId { get; set; }
        public string? TenBacSi { get; set; }
        public double SoSaoTrungBinh { get; set; } = 0;
        public int SoLuotDanhGia { get; set; } = 0;
        public int TongSoBacSi { get; set; } = 0;
        public int ThuHang { get; set; } = 0;
    }
}
