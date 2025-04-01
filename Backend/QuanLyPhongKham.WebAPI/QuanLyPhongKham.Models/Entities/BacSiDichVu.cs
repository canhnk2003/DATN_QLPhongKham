using QuanLyPhongKham.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Models.Entities
{
    public class BacSiDichVu
    {
        public Guid BacSiId { get; set; }
        public Guid DichVuId { get; set; }
        public decimal? Gia
        {
            get
            {
                switch(BacSi.BangCap)
                {
                    case (TrinhDo.GiaoSuYKhoa):
                        return (DichVu.DonGia + (DichVu.DonGia * 60 / 100));
                    case (TrinhDo.PhoGiaoSuYKhoa):
                        return (DichVu.DonGia + (DichVu.DonGia * 50 / 100));
                    case (TrinhDo.TienSiYKhoa):
                        return (DichVu.DonGia + (DichVu.DonGia * 40 / 100));
                    case (TrinhDo.BacSiChuyenKhoa2):
                        return (DichVu.DonGia + (DichVu.DonGia * 30 / 100));
                    case (TrinhDo.ThacSiYKhoa):
                        return (DichVu.DonGia + (DichVu.DonGia * 20 / 100));
                    case (TrinhDo.BacSiChuyenKhoa1):
                        return (DichVu.DonGia + (DichVu.DonGia * 10 / 100));
                    case (TrinhDo.BacSiDaKhoa):
                        return (DichVu.DonGia + (DichVu.DonGia * 0 / 100));
                    default:
                        return 0;
                }
            }
        }
        public BacSi BacSi { get; set; }
        public DichVu DichVu { get; set; }
    }
}
