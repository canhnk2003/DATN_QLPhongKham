using QuanLyPhongKham.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Business.Interfaces
{
    public interface IHomeService
    {
        /// <summary>
        /// Lấy ra thông tin phòng khám
        /// </summary>
        /// <returns></returns>
        Task<ClinicInfor> GetClinicInfor(Guid? doctorId = null);

        /// <summary>
        /// Thống kê lịch khám theo năm
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ThongKeLichKhamTheoNamModel>> ThongKeLichKhamTheoNamAsync(Guid? doctorId = null);

        /// <summary>
        /// Thống kê lịch khám theo trạng thái
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ThongKeLichKhamTheoTrangThaiModel>> ThongKeLichKhamTheoTrangThaiAsync(Guid? doctorId = null);

        /// <summary>
        /// Thống kê dịch vụ khám theo năm
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ThongKeDichVuTheoNamModel>> ThongKeDichVuPhoBienTheoNamAsync();

    }
}
