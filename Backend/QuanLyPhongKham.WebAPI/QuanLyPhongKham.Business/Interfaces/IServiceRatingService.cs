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
    }
}
