using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Data.Context;
using QuanLyPhongKham.Data.Interfaces;
using QuanLyPhongKham.Data.Repositories;
using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Business.Services
{
	public class ServiceService : BaseService<DichVu>, IServiceService
	{
		private readonly IServiceRepository _serviceRepository;
		private readonly ApplicationDbContext _context;

		public ServiceService(IServiceRepository serviceRepository, ApplicationDbContext context) : base(serviceRepository)
		{
			_serviceRepository = serviceRepository;
			_context = context;
		}

		public async Task<Khoa> GetKhoaByIdAsync(Guid khoaId)
		{
			return await _context.Khoas.FindAsync(khoaId);
		}

		// Phương thức AddAsync chỉ cần tùy chỉnh nếu bạn có logic kiểm tra dữ liệu riêng.
		public override async Task<int> AddAsync(DichVu entity)
		{
			// Kiểm tra tính hợp lệ của dữ liệu
			var checkData = _serviceRepository.CheckDataValidateForInsert(entity);
			if (checkData != null && checkData.Count > 0)
			{
				// Nếu có lỗi, ném ngoại lệ với thông báo lỗi
				throw new ErrorValidDataException(checkData);
			}
			else
			{
				// Tạo ID mới cho dịch vụ và thiết lập các trạng thái cần thiết
				entity.DichVuId = Guid.NewGuid();
				entity.MaDichVu = _serviceRepository.GetNextMaDichVu();
				entity.NgayTao = DateTime.Now;
				entity.NgayCapNhat = DateTime.Now;

				// Thêm dịch vụ vào cơ sở dữ liệu
				int res = await _serviceRepository.AddAsync(entity);

				// Kiểm tra nếu thêm thành công
				if (res > 0)
				{
					return res;  // Trả về kết quả thành công
				}
				else
				{
					// Nếu không thành công, ném ngoại lệ tạo mới thất bại
					throw new ErrorCreateException();
				}
			}
		}

		// Phương thức UpdateAsync chỉ cần tùy chỉnh nếu bạn có logic kiểm tra dữ liệu riêng.
		public override async Task<int> UpdateAsync(DichVu entity)
		{
			var checkData = _serviceRepository.CheckDataValidate(entity);

			//Nếu hợp lệ thì cho thêm, không thì lỗi
			if (checkData.Count > 0)
			{
				throw new ErrorValidDataException(checkData);
			}
			else
			{
				int res = await _serviceRepository.UpdateAsync(entity);

				if (res > 0)
				{
					return res;
				}
				else
				{
					throw new ErrorEditException();
				}
			}
		}

        public async Task<IEnumerable<DichVu>> GetByKhoaId(Guid khoaId)
        {
            return await _serviceRepository.GetByKhoaId(khoaId);
        }
    }
}
