using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Models.Exceptions
{
    /// <summary>
    /// Lỗi khi tạo không thành công
    /// </summary>
    /// Created By: NDChung - 13/10/2024
    public class ErrorCreateException : Exception
    {
        /// <summary>
        /// Ghi đè Thông báo lỗi
        /// </summary>
        public override string Message
        {
            get
            {
                return Resources.ResourceVN.Error_Create;
            }
        }
    }
}
