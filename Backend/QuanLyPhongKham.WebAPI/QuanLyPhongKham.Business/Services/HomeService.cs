using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Data.Interfaces;
using QuanLyPhongKham.Data.Repositories;
using QuanLyPhongKham.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Business.Services
{
    public class HomeService : IHomeService
    {
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IPatientRepository _patientRepo;
        private readonly IServiceRatingRepository _ratingRepo;
        private readonly IDoctorRepository _doctorRepo;
        private readonly IDepartmentRepository _departmentRepo;
        public HomeService(IAppointmentRepository appointmentRepo, IPatientRepository patientRepo, IDoctorRepository doctorRepo, IServiceRatingRepository serviceRatingRepository, IDepartmentRepository departmentRepository)
        {
            _appointmentRepo = appointmentRepo;
            _patientRepo = patientRepo;
            _doctorRepo = doctorRepo;
            _ratingRepo = serviceRatingRepository;
            _departmentRepo = departmentRepository;
        }
        public async Task<ClinicInfor> GetClinicInfor()
        {
            //Lấy ra danh sách lịch khám
            var appointments = await _appointmentRepo.GetAllAsync();

            //Lấy ra danh sách bệnh nhân
            var patients = await _patientRepo.GetAllAsync();

            //Tổng lịch khám
            int sumApp = appointments.Count();

            //Tổng lịch khám thành công
            int sumAppSuc = appointments.Where(x => x.TrangThaiLichKham == "Hoàn thành" 
                            || x.TrangThaiLichKham == "Đã hoàn thành").Count();
            
            //Tổng lịch khám hủy
            int sumAppCan = appointments.Where(x => x.TrangThaiLichKham == "Đã hủy").Count();

            //Tổng số bệnh nhân
            int sumPat = patients.Count();

            ClinicInfor clinic = new ClinicInfor();
            clinic.SumAppointment = sumApp;
            clinic.SumAppointmentSuccess = sumAppSuc;
            clinic.SumAppointmentCancel = sumAppCan;
            clinic.SumPatient = sumPat;

            return clinic;
        }
    }
}
