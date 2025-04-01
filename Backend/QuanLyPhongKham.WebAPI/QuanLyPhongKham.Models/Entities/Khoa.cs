using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Models.Entities
{
    public class Khoa
    {
        public Guid KhoaId { get; set; }
        public string MaKhoa { get; set; }
        public string TenKhoa { get; set; }
        public DateTime? NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; } = DateTime.Now;
        public virtual ICollection<DichVu>? DichVus { get; set; }
        public virtual ICollection<BacSi>? BacSis { get; set; }
    }
}
