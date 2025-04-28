using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Business.Interfaces
{
	public interface IServiceService : IBaseService<DichVu>
	{
		// Lấy tất cả các dịch vụ
		Task<Khoa> GetKhoaByIdAsync(Guid khoaId);
        // Các phương thức bổ sung tùy chỉnh nếu cần, ví dụ tìm kiếm dịch vụ theo tên

        Task<IEnumerable<DichVu>> GetByKhoaId(Guid khoaId);

		/// <summary>
		/// Dịch vụ thêm dữ liệu từ excel
		/// </summary>
		/// <param name="dichVuModels"></param>
		/// <returns></returns>
		Task<int> ImportDataFromExcel(List<DichVu> dichVuModels);
    }
}
