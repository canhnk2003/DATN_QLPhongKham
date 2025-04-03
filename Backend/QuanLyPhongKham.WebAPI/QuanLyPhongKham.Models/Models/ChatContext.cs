using QuanLyPhongKham.Models.Entities;
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
        public Guid? PatientId { get; set; }
        public string? PatientName { get; set; }
        public string? PatientPhone { get; set; }
        public string? PatientEmail { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string? AppointmentTime { get; set; }
        public Guid? DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public Guid? ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public string? Reason { get; set; }
        public bool IsReadyForBooking { get; set; } = false;
    }
}
