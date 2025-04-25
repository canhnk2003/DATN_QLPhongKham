using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Business.Interfaces
{
    public interface IAppointmentService:IBaseService<LichKham>
    {
        Task<IEnumerable<AppointmentModel>> GetAppointment();
        /// <summary>
        /// Lấy danh sách lịch khám theo id
        /// </summary>
        /// <param name="bacSiId">id</param>
        /// <returns>danh sách</returns>
        Task<IEnumerable<LichKham>> GetAppointmentsByDoctor(Guid bacSiId);
        /// <summary>
        /// Lấy danh sách lịch khám theo benhNhanId
        /// </summary>
        /// <param name="benhNhanId">id</param>
        /// <returns></returns>
        Task<IEnumerable<LichKham>> GetAppointmentsByPatient(Guid benhNhanId);
        Task<int> CancelAppointment(Guid id, string? lyDo);
        Task<LichKham>? GetLichKhamLatest(Guid benhNhanId);

        Task<int> EditAsync(LichKham lichKham, Guid id);
        Task<int> AcceptAppointment(Guid id);
        Task<int> CompleteAppointment(Guid LichKhamId);
        Task<int> Complete(Guid LichKhamId);
    }
}
