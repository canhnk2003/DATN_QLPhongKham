using QuanLyPhongKham.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Business.Interfaces
{
    public interface IPatientService : IBaseService<BenhNhan>
    {
        Task<IEnumerable<BenhNhan>> GetAllByDoctorIdAsync(Guid BacSiId);
        Task<BenhNhan> GetByUserId(string userId);
    }
}
