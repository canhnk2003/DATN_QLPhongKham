using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Data.Interfaces
{
    public interface IResultRepository : IBaseRepository<KetQuaKham>
    {

        // Thêm phương thức để lấy kết quả khám theo bác sĩ
        IEnumerable<KetQuaKham> GetAllByDoctorId(Guid bacSiId);
        Dictionary<string, string>? CheckDataValidate(KetQuaKham ketQuaKham);
        Dictionary<string, string>? CheckDataValidateForInsert(KetQuaKham ketQuaKham);

        KetQuaKham? GetKetQuaKhamByLichKhamId(Guid lichKhamId);
        string GetBenhNhanNameByLichKhamId(Guid lichKhamId);
        IEnumerable<KetQuaKhamModel> GetAllByPatientId(Guid benhNhanId);

    }
}
