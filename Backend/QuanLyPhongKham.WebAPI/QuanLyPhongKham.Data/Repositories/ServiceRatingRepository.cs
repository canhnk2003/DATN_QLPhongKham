using QuanLyPhongKham.Data.Context;
using QuanLyPhongKham.Data.Interfaces;
using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Data.Repositories
{
    public class ServiceRatingRepository : BaseRepository<DanhGiaDichVu>, IServiceRatingRepository
    {
        public ServiceRatingRepository(ApplicationDbContext context) : base(context)
        {
        }

    }
}
