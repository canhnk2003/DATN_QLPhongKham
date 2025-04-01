using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Business.Services;
using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Models;

namespace QuanLyPhongKham.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public DoctorsController(IDoctorService doctorService, IMapper mapper, IAuthService authService)
        {
            _doctorService = doctorService;
            _mapper = mapper;
            _authService = authService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllDoctor()
        {
            var doctors = await _doctorService.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<BacSiModel>>(doctors));
        }

        [HttpGet("{bacSiId}")]
        public async Task<IActionResult> GetDoctorById(Guid bacSiId)
        {
            //Lấy dữ liệu
            var doctorById = await _doctorService.GetByIdAsync(bacSiId);
            return Ok(_mapper.Map<BacSiModel>(doctorById));
        }

        [HttpGet("getbyuserid/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var bs = await _doctorService.GetByUserId(userId);
            return Ok(_mapper.Map<BacSiModel>(bs));

        }

        [HttpGet("doctor/{departmentId}")]
        public async Task<IActionResult> GetDoctorsByDepartmentId(Guid departmentId)
        {
            var doctors = await _doctorService.GetBacSisByKhoaId(departmentId);
            return Ok(_mapper.Map<IEnumerable<BacSiModel>>(doctors));
        }

        [HttpPost]
        public async Task<IActionResult> AddDoctor([FromBody] BacSiModel bacSi)
        {
            //int res = await _doctorService.AddAsync(_mapper.Map<BacSi>(bacSi));
            //return StatusCode(201, res);
            if (bacSi == null)
            {
                return BadRequest(new { message = "Invalid client request" });
            }

            // Tạo tài khoản cho bác sĩ
            RegisterModel registerModel = new RegisterModel();
            
            registerModel.Email = bacSi.Email;
            registerModel.Username = bacSi.Email;
            registerModel.Password = "Doctor@123"; // Password được truyền từ client trong BacSiModel

            // Kiểm tra _authService có null không
            //if (_authService == null)
            //{
            //    return StatusCode(500, new { message = "Auth service not initialized" });
            //}

            var accountResult = await _authService.RegisterDoctorAsync(registerModel);

            // Kiểm tra accountResult có null không và trạng thái có đúng không
            if (accountResult == null || accountResult.Status != "Success")
            {
                // Trả về một phản hồi lỗi với thông báo cụ thể
                return BadRequest(new { message = "Account creation failed", details = accountResult });
            }

            // Thêm thông tin bác sĩ vào hệ thống
            var bacSiEntity = _mapper.Map<BacSi>(bacSi); // Đổi tên biến cục bộ
            bacSiEntity.UserId = accountResult.Data.ToString(); // Liên kết UserId từ tài khoản mới tạo

            int res = await _doctorService.AddAsync(bacSiEntity);

            return StatusCode(201, new { DoctorId = res, Message = "Doctor and account created successfully" });
        }

        [HttpPut("{bacSiId}")]
        public async Task<IActionResult> UpdateDoctor(Guid bacSiId, [FromBody] BacSiModel bacSi)
        {
            if (bacSiId != bacSi.BacSiId)
            {
                return BadRequest("Id không giống!");
            }
            var existingBS = await _doctorService.GetByIdAsync(bacSiId);
            existingBS.HoTen = bacSi.HoTen;
            existingBS.KhoaId = bacSi.KhoaId;
            existingBS.SoDienThoai = bacSi.SoDienThoai;
            existingBS.Email = bacSi.Email;
            existingBS.DiaChi = bacSi.DiaChi;
            existingBS.BangCap = bacSi.BangCap;
            existingBS.SoNamKinhNghiem = bacSi.SoNamKinhNghiem;
            existingBS.GioLamViec = bacSi.GioLamViec;
            if (bacSi.HinhAnh != null)
            {
                existingBS.HinhAnh = bacSi.HinhAnh;
            }
            int res = await _doctorService.UpdateAsync(existingBS);
            return StatusCode(204, res);
        }

        [HttpDelete("{bacSiId}")]
        public async Task<IActionResult> DeleteDoctor(Guid bacSiId)
        {
            // Kiểm tra xem bác sĩ có tồn tại không
            var bs = await _doctorService.GetByIdAsync(bacSiId);
            if (bs == null)
            {
                // Không tìm thấy bác sĩ, trả về lỗi
                return NotFound();
            }

            // Thực hiện xóa bác sĩ
            var res = await _doctorService.DeleteAsync(bacSiId);
            if (bs.UserId != null)
            {
                var user = await _authService.FindByIdAsync(bs.UserId);
                if (user != null)
                {
                    await _authService.DeleteUser(bs.UserId);
                }
                if (res > 0)
                {
                    // Xóa thành công
                    return StatusCode(201, res);
                }
                else
                {
                    // Nếu có lỗi xảy ra khi xóa, trả về mã lỗi
                    return StatusCode(500);
                }
            }
            return Ok();
            
        }
    }
}
