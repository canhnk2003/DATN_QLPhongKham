using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data.Context;
using QuanLyPhongKham.Data.Interfaces;
using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Resources;
using QuanLyPhongKham.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Data.Repositories
{
    public class AppointmentRepository : BaseRepository<LichKham>, IAppointmentRepository
    {
        public AppointmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LichKham>>? GetAppointmentsByDoctor(Guid bacSiId)
        {
            var appointments = await _context.LichKhams.Where(a => a.BacSiId == bacSiId).ToListAsync();
            return appointments;
        }

        public async Task<IEnumerable<LichKham>>? GetAppointmentsByPatient(Guid benhNhanId)
        {
            var appointments = await _context.LichKhams.Where(a => a.BenhNhanId == benhNhanId).ToListAsync();
            return appointments;
        }

        public async Task<LichKham>? GetLichKhamLatest(Guid benhNhanId)
        {
            var lichKham = await _context.LichKhams.OrderByDescending(l => l.NgayCapNhat).FirstOrDefaultAsync();
            return lichKham;
        }

        public async Task<int> CountAppointmentsAsync(DateTime ngayKham, string gioKham)
        {
            return await _context.LichKhams.CountAsync(l => l.NgayKham == ngayKham &&
            l.GioKham == gioKham && (l.TrangThaiLichKham == "Đang xử lý" || l.TrangThaiLichKham == "Đã đặt"));
        }

        public async Task<List<LichKham>> GetLichKhamByDateAndTimeAsync(DateTime ngayKham, string gioKham)
        {
            return await _context.LichKhams.Where(l => l.NgayKham == ngayKham && l.GioKham == gioKham).ToListAsync();
        }
    }
}
