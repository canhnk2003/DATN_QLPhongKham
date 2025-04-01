using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Models.Models
{
    public class ChangePasswordModel
    {
        public string username { get; set; }
        public string currentPassword { get; set; }
        public string newPassword { get; set; }
    }
}
