using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Data.Context;
using QuanLyPhongKham.Data.Interfaces;
using QuanLyPhongKham.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Data.Repositories
{
	public class ServiceRepository : BaseRepository<DichVu>, IServiceRepository
	{
		public ServiceRepository(ApplicationDbContext context) : base(context)
		{
		}

		// Kiểm tra dữ liệu hợp lệ khi sửa hoặc thêm dịch vụ
		public Dictionary<string, string>? CheckDataValidate(DichVu dichVu)
		{
			var errors = new Dictionary<string, string>();

			if (string.IsNullOrEmpty(dichVu.TenDichVu))
				errors.Add("TenDichVu", "Tên dịch vụ không được để trống");

			if (dichVu.DonGia <= 0)
				errors.Add("DonGia", "Đơn giá phải lớn hơn 0");

			return errors;
		}

		public Dictionary<string, string>? CheckDataValidateForInsert(DichVu dichVu)
		{
			var errors = new Dictionary<string, string>();

			if (string.IsNullOrEmpty(dichVu.TenDichVu))
				errors.Add("TenDichVu", "Tên dịch vụ không được để trống");

			if (string.IsNullOrEmpty(dichVu.MaDichVu))
				errors.Add("MaDichVu", "Mã dịch vụ không được để trống");

			if (dichVu.DonGia <= 0)
				errors.Add("DonGia", "Đơn giá phải lớn hơn 0");

			return errors;
		}

        public async Task<IEnumerable<DichVu>> GetByKhoaId(Guid khoaId)
        {
            var dichVus = await _context.DichVus.Where(x=>x.KhoaId == khoaId).ToListAsync();
			return dichVus;
        }

        public string GetNextMaDichVu()
		{
			// Lấy danh sách mã bệnh nhân
			var maxMaDV = _context.DichVus
				.AsEnumerable() // Chuyển sang client side
				.Select(dv => new
				{
					MaBN = dv.MaDichVu,
					So = int.Parse(dv.MaDichVu.Substring(2)) // Tách phần số, bỏ qua 2 ký tự đầu tiên "BN"
				})
				.OrderByDescending(dv => dv.So) // Sắp xếp theo số, giảm dần
				.FirstOrDefault();

			// Nếu không có bệnh nhân nào trong hệ thống, mã bắt đầu từ "BN001"
			if (maxMaDV == null)
			{
				return "DV001";
			}

			// Lấy phần số lớn nhất và cộng thêm 1
			int nextSo = maxMaDV.So + 1;

			// Kết hợp phần chữ "DV" và phần số (cộng thêm 1), định dạng phần số với 3 chữ số
			return $"DV{nextSo:D3}";
		}
	}
}
