using QuanLyPhongKham.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Data.Interfaces
{
    public interface IAppointmentRepository:IBaseRepository<LichKham>
    {
        /// <summary>
        /// Lấy ra danh sách lịch khám theo BacSiId
        /// </summary>
        /// <param name="bacSiId">id</param>
        /// <returns>Danh sách lịch khám</returns>
        Task<IEnumerable<LichKham>>? GetAppointmentsByDoctor(Guid bacSiId);
        /// <summary>
        /// Lấy ra danh sách lịch khám theo benhNhanId
        /// </summary>
        /// <param name="benhNhanId">id</param>
        /// <returns>Danh sách lịch khám</returns>
        Task<IEnumerable<LichKham>>? GetAppointmentsByPatient(Guid benhNhanId);

        /// <summary>
        /// Lấy lịch khám gần nhất
        /// </summary>
        /// <param name="benhNhanId">id</param>
        /// <returns>Lịch khám</returns>
        Task<LichKham>? GetLichKhamLatest(Guid benhNhanId);

        Task<List<LichKham>> GetLichKhamByDateAndTimeAsync(DateTime ngayKham, string gioKham);
    }
}
