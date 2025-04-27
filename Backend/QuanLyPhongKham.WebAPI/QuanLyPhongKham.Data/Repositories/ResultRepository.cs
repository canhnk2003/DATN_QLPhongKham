using QuanLyPhongKham.Data.Context;
using QuanLyPhongKham.Data.Interfaces;
using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Data.Repositories
{
    public class ResultRepository : BaseRepository<KetQuaKham>, IResultRepository
    {
        public ResultRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Dictionary<string, string>? CheckDataValidate(KetQuaKham ketQuaKham)
        {
            var errors = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(ketQuaKham.ChanDoan))
                errors.Add("ChanDoan", "Chẩn đoán không được để trống");

            
            return errors;
        }


        public Dictionary<string, string>? CheckDataValidateForInsert(KetQuaKham ketQuaKham)
        {
            var errors = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(ketQuaKham.ChanDoan))
                errors.Add("ChanDoan", "Chẩn đoán không được để trống");

            return errors;
        }

        public KetQuaKham? GetKetQuaKhamByLichKhamId(Guid lichKhamId)
        {
            var kq = _context.KetQuaKhams.Where(k => k.LichKhamId == lichKhamId).FirstOrDefault();
            return kq;
        }

        // Phương thức lấy kết quả khám của bác sĩ thông qua LichKham
        public IEnumerable<KetQuaKham> GetAllByDoctorId(Guid bacSiId)
        {
            var results = _context.KetQuaKhams
                .Join(_context.LichKhams,
                    kq => kq.LichKhamId,
                    lk => lk.LichKhamId,
                    (kq, lk) => new { KQ = kq, LK = lk })
                .Where(x => x.LK.BacSiId == bacSiId)  // Lọc theo BacSiId trong bảng LichKham
                .Select(x => x.KQ)
                .ToList();

            return results;
        }

        public string GetBenhNhanNameByLichKhamId(Guid lichKhamId)
        {
            // Truy vấn kết quả khám theo LichKhamId
            var lichKham = _context.LichKhams
                .Where(lk => lk.LichKhamId == lichKhamId)
                .FirstOrDefault();

            if (lichKham != null)
            {
                // Lấy BenhNhanId từ LichKham
                var benhNhanId = lichKham.BenhNhanId;

                // Truy vấn tên bệnh nhân theo BenhNhanId
                var benhNhan = _context.BenhNhans
                    .Where(bn => bn.BenhNhanId == benhNhanId)
                    .FirstOrDefault();

                if (benhNhan != null)
                {
                    return benhNhan.HoTen;  // Trả về tên bệnh nhân
                }
            }

            return null;  // Nếu không tìm thấy tên bệnh nhân
        }

        public IEnumerable<KetQuaKhamModel> GetAllByPatientId(Guid benhNhanId)
        {
            //var results = _context.KetQuaKhams
            //        .Where(k => k.LichKham.BenhNhanId == benhNhanId)
            //        .ToList();
            var results = (from kq in _context.KetQuaKhams
                           join lk in _context.LichKhams on kq.LichKhamId equals lk.LichKhamId
                           join bs in _context.BacSis on lk.BacSiId equals bs.BacSiId
                           join bn in _context.BenhNhans on lk.BenhNhanId equals bn.BenhNhanId
                           join dv in _context.DichVus on lk.DichVuId equals dv.DichVuId into dvGroup
                           from dv in dvGroup.DefaultIfEmpty() // left join
                           where bn.BenhNhanId == benhNhanId
                           orderby lk.NgayKham descending
                           select new KetQuaKhamModel
                           {
                               KetQuaKhamId = kq.KetQuaKhamId,
                               ChanDoan = kq.ChanDoan,
                               ChiDinhThuoc = kq.ChiDinhThuoc,
                               GhiChu = kq.GhiChu,
                               NgayKham = lk.NgayKham ?? DateTime.MinValue,
                               GioKham = lk.GioKham,
                               BacSi = new BacSiModel
                               {
                                   HoTen = bs.HoTen
                               },
                               BenhNhan = new BenhNhanModel
                               {
                                   BenhNhanId = bn.BenhNhanId,
                                   MaBenhNhan = bn.MaBenhNhan,
                                   HoTen = bn.HoTen,
                                   NgaySinh = bn.NgaySinh,
                                   SoDienThoai = bn.SoDienThoai,
                                   DiaChi = bn.DiaChi,
                                   Email = bn.Email,
                                   LoaiGioiTinh = bn.LoaiGioiTinh,
                                   TienSuBenhLy = bn.TienSuBenhLy
                               },
                               DichVu = dv == null ? null : new DichVuModel
                               {
                                   TenDichVu = dv.TenDichVu
                               }
                           }).ToList();

            return results;
        }

    }
}