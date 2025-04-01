using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Models.Exceptions
{
    /// <summary>
    /// Lỗi khi sửa không thành công
    /// </summary>
    /// Created By: NDChung - 13/10/2024
    public class ErrorEditException : Exception
    {
        //Ghi đè thông báo lỗi
        public override string Message
        {
            get
            {
                return Resources.ResourceVN.Error_Edit;
            }
        }
    }
}
