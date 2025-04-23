using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Data.Interfaces
{
    public interface IServiceRatingRepository:IBaseRepository<DanhGiaDichVu>
    {
        //Task<DanhGiaDichVu>? GetRatingByAppointmentId(Guid id);
    }
}
