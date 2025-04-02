using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Models.Models
{
    // Lớp lưu ngữ cảnh cuộc trò chuyện
    public class ChatContext
    {
        public string LastTopic { get; set; } = "";
        public string LastSpecialization { get; set; } = "";
    }
}
