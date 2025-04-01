using QuanLyPhongKham.Models.Entities;
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
    }
}
