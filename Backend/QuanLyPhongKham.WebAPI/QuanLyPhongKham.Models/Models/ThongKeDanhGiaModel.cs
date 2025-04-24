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
        public string BangCap { get; set; } = string.Empty;
        public string MaBacSi { get; set; } = string.Empty;
        public int SoNamKinhNghiem { get; set; } = 0;
        public string TenKhoa { get; set; } = string.Empty;
    }
}
