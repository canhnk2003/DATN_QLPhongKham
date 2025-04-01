using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Models.Entities
{
    public class Service
    {
        /// <summary>
        /// Thông báo cho dev
        /// </summary>
        public string? DevMsg { get; set; }

        /// <summary>
        /// Thông báo cho người dùng
        /// </summary>
        public string? UserMsg { get; set; }

        /// <summary>
        /// Dữ liệu trả về
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// Danh sách lỗi
        /// </summary>
        public Dictionary<string, string>? Error { get; set; }
    }
}
