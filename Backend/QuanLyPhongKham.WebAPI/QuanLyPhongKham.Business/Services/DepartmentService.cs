using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Data.Interfaces;
using QuanLyPhongKham.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Business.Services
{
    public class DepartmentService : BaseService<Khoa>, IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(IBaseRepository<Khoa> repository, IDepartmentRepository departmentRepository) : base(repository)
        {
            _departmentRepository = departmentRepository;
        }
    }
}
