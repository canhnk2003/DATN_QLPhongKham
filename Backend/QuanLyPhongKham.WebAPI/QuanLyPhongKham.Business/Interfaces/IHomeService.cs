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
        /// Lấy ra tổng số lượng lịch khám
        /// </summary>
        /// <returns></returns>
        Task<ClinicInfor> GetClinicInfor();
        //Task<IEnumerable<ThongKeDanhGiaModel>> GetAllDoctorInfor();
    }
}
