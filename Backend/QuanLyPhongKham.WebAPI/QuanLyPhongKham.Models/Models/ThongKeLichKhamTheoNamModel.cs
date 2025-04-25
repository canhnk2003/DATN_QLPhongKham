using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Models.Models
{
    public class ThongKeLichKhamTheoNamModel
    {
        public int Nam { get; set; }
        public int[] SoLuongTheoThang { get; set; } = new int[12]; // 12 tháng
    }
}
