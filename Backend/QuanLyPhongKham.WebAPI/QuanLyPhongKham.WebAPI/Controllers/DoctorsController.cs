using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Business.Services;
using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Enums;
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
            if (bacSi == null)
            {
                return BadRequest(new { message = "Invalid client request" });
            }

            // Tạo tài khoản cho bác sĩ
            RegisterModel registerModel = new RegisterModel();

            registerModel.Email = bacSi.Email;
            registerModel.Username = bacSi.Email;
            registerModel.Password = "Doctor@123"; // Password được truyền từ client trong BacSiModel

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

        [Authorize(Roles = "Admin")]
        [HttpPost("import-bacsi")]
        public async Task<IActionResult> ImportDataFromExcel(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                return BadRequest("Vui lòng chọn file excel để upload!");
            }
            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("File phải có định dạng .xlsx!");
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var listBS = new List<BacSiModel>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                        return BadRequest(new { message = "No worksheet found" });

                    int rowCount = worksheet.Dimension.Rows;
                    for (int row = 2; row <= rowCount; row++) // bắt đầu từ dòng 2
                    {
                        try
                        {
                            if (string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Text)) continue; // Dòng trống

                            var doctor = new BacSiModel
                            {
                                KhoaId = Guid.TryParse(worksheet.Cells[row, 2].Text, out Guid khoaId) ? khoaId : (Guid?)null,
                                MaBacSi = worksheet.Cells[row, 3].Text,
                                HoTen = worksheet.Cells[row, 4].Text,
                                Email = worksheet.Cells[row, 5].Text,
                                SoDienThoai = worksheet.Cells[row, 6].Text,
                                SoNamKinhNghiem = ConvertKinhNghiem(worksheet.Cells[row, 7].Text),
                                BangCap = ConvertBangCap(worksheet.Cells[row, 8].Text),
                                GioLamViec = worksheet.Cells[row, 9].Text,
                                DiaChi = worksheet.Cells[row, 10].Text
                            };

                            listBS.Add(doctor);
                        }
                        catch (Exception ex)
                        {
                            continue; // Skip lỗi dòng này
                        }
                    }
                }
            }

            int count = 0;
            // Gọi lần lượt AddDoctor cho từng bác sĩ
            foreach (var doctor in listBS)
            {
                try
                {
                    var result = await AddDoctor(doctor);
                    if(result != null)
                    {
                        count++;
                    }
                    if (result is BadRequestObjectResult badRequest)
                    {
                        // Xử lý lỗi nếu cần
                        continue;
                    }
                }
                catch(Exception ex)
                {
                    continue;
                }
            }

            return Ok(new { success = true, message = $"{count} bản ghi được thêm" });
        }
        private int? ConvertKinhNghiem(string kinhNghiemText)
        {
            if (string.IsNullOrWhiteSpace(kinhNghiemText)) return null;

            var numberPart = new string(kinhNghiemText.TakeWhile(char.IsDigit).ToArray());
            if (int.TryParse(numberPart, out int years))
            {
                return years;
            }

            return null;
        }
        private TrinhDo? ConvertBangCap(string bangCapText)
        {
            if (string.IsNullOrWhiteSpace(bangCapText)) return null;

            switch (bangCapText.Trim())
            {
                case "Giáo sư Y khoa":
                    return TrinhDo.GiaoSuYKhoa;
                case "Phó Giáo sư Y khoa":
                    return TrinhDo.PhoGiaoSuYKhoa;
                case "Tiến sĩ Y khoa":
                    return TrinhDo.TienSiYKhoa;
                case "Bác sĩ Chuyên khoa 2":
                    return TrinhDo.BacSiChuyenKhoa2;
                case "Thạc sĩ Y khoa":
                    return TrinhDo.ThacSiYKhoa;
                case "Bác sĩ Chuyên khoa 1":
                    return TrinhDo.BacSiChuyenKhoa1;
                case "Bác sĩ Đa khoa":
                    return TrinhDo.BacSiDaKhoa;
                default:
                    return null;
            }
        }

    }
}
