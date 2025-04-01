using QuanLyPhongKham.Data.Context;
using QuanLyPhongKham.Data.Interfaces;
using QuanLyPhongKham.Models.Entities;
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

        public IEnumerable<KetQuaKham> GetAllByPatientId(Guid benhNhanId)
        {
            var results = _context.KetQuaKhams
                    .Where(k => k.LichKham.BenhNhanId == benhNhanId)
                    .ToList();

            return results;
        }
    }
}