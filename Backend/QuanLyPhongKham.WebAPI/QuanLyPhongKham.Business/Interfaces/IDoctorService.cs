using QuanLyPhongKham.Data.Interfaces;
using QuanLyPhongKham.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Business.Interfaces
{
    public interface IDoctorService : IBaseService<BacSi>
    {
        Task<IEnumerable<BacSi>> GetBacSisByKhoaId(Guid id);
        Task<BacSi> GetByUserId(string userId);
        

    }
}
