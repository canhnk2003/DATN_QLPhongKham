using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Exceptions;
using QuanLyPhongKham.Models.Models;

namespace QuanLyPhongKham.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public PatientsController(IPatientService patientService, IMapper mapper, IAuthService authService)
        {
            _patientService = patientService;
            _mapper = mapper;
            _authService = authService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAllPatient() 
        { 
            var patients = await _patientService.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<BenhNhanModel>>(patients));
        }

        [HttpGet("{benhNhanId}")]
        public async Task<IActionResult> GetPatientById(Guid benhNhanId)
        {
            //Lấy dữ liệu
            var bn = await _patientService.GetByIdAsync(benhNhanId);
            return Ok(_mapper.Map<BenhNhanModel>(bn));
        }

        [HttpGet("getbyuserid/{userId}")]
        public async Task<IActionResult> GetPatientByUserId(string userId)
        {
            //Lấy dữ liệu
            var bn = await _patientService.GetByUserId(userId);
            return Ok(_mapper.Map<BenhNhanModel>(bn));
        }

        [HttpGet("getbydoctorid/{bacSiId}")]
        public async Task<IActionResult> GetAllByDoctorId(Guid bacSiId)
        {
            var patients = await _patientService.GetAllByDoctorIdAsync(bacSiId);
            return Ok(_mapper.Map<IEnumerable<BenhNhanModel>>(patients));
        }

        [HttpPost]
        public async Task<IActionResult> AddPatient([FromBody] BenhNhanModel benhNhan)
        {
            int res = await _patientService.AddAsync(_mapper.Map<BenhNhan>(benhNhan));
            return StatusCode(201, res);
        }

        [HttpPut("{benhNhanId}")]
        public async Task<IActionResult> UpdatePatient(Guid benhNhanId, [FromBody] BenhNhanModel benhNhan)
        {
            if(benhNhanId != benhNhan.BenhNhanId)
            {
                return BadRequest("Id không giống!");
            }
            var existingBN = await _patientService.GetByIdAsync(benhNhanId);
            existingBN.HoTen = benhNhan.HoTen;
            existingBN.NgaySinh = benhNhan.NgaySinh;
            existingBN.LoaiGioiTinh = benhNhan.LoaiGioiTinh;
            existingBN.SoDienThoai = benhNhan.SoDienThoai;
            existingBN.Email = benhNhan.Email;
            existingBN.DiaChi = benhNhan.DiaChi;
            existingBN.TienSuBenhLy = benhNhan.TienSuBenhLy;
            if (benhNhan.HinhAnh != null)
            {
                existingBN.HinhAnh = benhNhan.HinhAnh;
            }
            int res = await _patientService.UpdateAsync(existingBN);
            return StatusCode(201, res);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{benhNhanId}")]
        public async Task<IActionResult> DeletePatient(Guid benhNhanId)
        {
            var bn = await _patientService.GetByIdAsync(benhNhanId);
            var res = await _patientService.DeleteAsync(benhNhanId);
            
            var user = await _authService.FindByIdAsync(bn.UserId);
            if (user != null)
            {
                await _authService.DeleteUser(bn.UserId);
            }
            return StatusCode(201, res);
        }
    }
}
