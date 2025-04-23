using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Business.Interfaces
{
    public interface IServiceRatingService:IBaseService<DanhGiaDichVu>
    {
        /// <summary>
        /// Sửa đánh giá
        /// </summary>
        /// <param name="danhGia">dữ liệu</param>
        /// <param name="id">danhGaId</param>
        /// <returns></returns>
        Task<int> EditAsync(DanhGiaDichVu danhGia, Guid id);
        /// <summary>
        /// Lấy ra danh sách đánh giá với tên bác sĩ, bệnh nhân
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DanhGiaDichvuModel>> GetAllWithNameAsync();

        /// <summary>
        /// Lấy ra danh sách đánh giá phân theo bác sĩ + số lượt đánh giá TB
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ThongKeDanhGiaModel>> GetAllAverageAsync();
        /// <summary>
        /// Lấy ra danh sách đánh giá theo bác sĩ
        /// </summary>
        /// <param name="doctorId"></param>
        /// <returns></returns>

        Task<IEnumerable<DanhGiaDichvuModel>> GetAllRatingByDoctor(Guid doctorId);

        /// <summary>
        /// Lấy ra 1 đánh giá theo bác sĩ id
        /// </summary>
        /// <param name="doctorId"></param>
        /// <returns></returns>
        Task<ThongKeDanhGiaModel> GetRatingByDoctor(Guid doctorId);

        /// <summary>
        /// Lấy ra đánh giá theo lichkhamid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //Task<DanhGiaDichVu> GetRatingByAppointmentId(Guid id);
    }
}
