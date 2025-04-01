using QuanLyPhongKham.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Models.Models
{
    public class ResultModel
    {
        public Guid KetQuaKhamId { get; set; }
        public Guid? LichKhamId { get; set; }
        public string ChanDoan { get; set; }
        public string? ChiDinhThuoc { get; set; }
        public string? GhiChu { get; set; }
        public DateTime? NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; } = DateTime.Now;
    }
}
