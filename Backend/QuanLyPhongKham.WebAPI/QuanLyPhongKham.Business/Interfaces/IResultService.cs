using QuanLyPhongKham.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Business.Interfaces
{
    public interface IResultService : IBaseService<KetQuaKham>
    {
        // Thêm phương thức để lấy kết quả khám theo bác sĩ ID
        Task<IEnumerable<KetQuaKham>> GetAllByDoctorIdAsync(Guid bacSiId);
        Task<IEnumerable<KetQuaKham>> GetAllByPatientIdAsync(Guid benhNhanId);
        KetQuaKham? GetKetQuaKhamByLichKhamId(Guid lichKhamId);
        string GetBenhNhanNameByLichKhamId(Guid lichKhamId);
    }
}
