using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Models.Models
{
    public class ThongKeDichVuTheoNamModel
    {
        public int Nam { get; set; }
        public List<DichVuPhanTramModel> TopDichVu { get; set; } = new();
    }
}
