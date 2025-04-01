using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Models.Exceptions
{
    public class ErrorChangePassException: Exception
    {
        public override string Message
        {
            get
            {
                return Resources.ResourceVN.Error_ChangePassword;
            }
        }
    }
}
