using QuanLyPhongKham.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Data.Interfaces
{
    public interface IDoctorRepository : IBaseRepository<BacSi>
    {
        string GetNextMaBacSi();
        Dictionary<string, string>? CheckDataValidate (BacSi bacSi);
        Dictionary<string, string>? CheckDataValidateForInsert(BacSi bacSi);
        Task<IEnumerable<BacSi>> GetBacSisByKhoaId(Guid id);
        Task<BacSi> GetByUserId(string userId);

    }
}
