using QuanLyPhongKham.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Data.Interfaces
{
	public interface IServiceRepository : IBaseRepository<DichVu>
	{
		Task<string> GetNextMaDichVu(Guid khoaId);
		Dictionary<string, string>? CheckDataValidate(DichVu dichVu);
		Dictionary<string, string>? CheckDataValidateForInsert(DichVu dichVu);
		Task<IEnumerable<DichVu>> GetByKhoaId(Guid khoaId);
	}
}
