using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Business.Services;
using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Models;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace QuanLyPhongKham.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceService _serviceService;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public ServicesController(IServiceService serviceService, IAuthService authService, IMapper mapper)
        {
            this._serviceService = serviceService;
            this._authService = authService;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> getDichVu()
        {
            var dichvus = await _serviceService.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<DichVuModel>>(dichvus));
        }

        [HttpGet("{DepartmentId}")]
        public async Task<IActionResult> GetByKhoaId(Guid DepartmentId)
        {
            var dichvus = await _serviceService.GetByKhoaId(DepartmentId);
            return Ok(dichvus);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddDichVu([FromBody] DichVuModel dichVuModel)
        {
            // Kiểm tra nếu model không hợp lệ
            if (dichVuModel == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            // Chuyển đổi từ DichVuModel sang DichVu entity
            var dichVuEntity = _mapper.Map<DichVu>(dichVuModel);

            try
            {
                // Thêm dịch vụ mới
                int result = await _serviceService.AddAsync(dichVuEntity);

                // Trả về kết quả thành công
                return CreatedAtAction(nameof(getDichVu), new { id = dichVuEntity.DichVuId }, dichVuEntity);
            }
            catch (Exception ex)
            {
                // Nếu có lỗi trong quá trình thêm
                return StatusCode(500, $"Lỗi khi thêm dịch vụ: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{dichVuId}")]
        public async Task<IActionResult> UpdateService(Guid dichVuId, [FromBody] DichVuModel dichVu)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dichVuId != dichVu.DichVuId)
            {
                return BadRequest("Id không giống!");
            }

            var existingDV = await _serviceService.GetByIdAsync(dichVuId);
            if (existingDV == null)
            {
                return BadRequest("Không tìm thấy dịch vụ");
            }
            existingDV.KhoaId = dichVu.KhoaId;
            existingDV.TenDichVu = dichVu.TenDichVu;
            existingDV.MoTaDichVu = dichVu.MoTaDichVu;
            existingDV.DonGia = dichVu.DonGia;
            existingDV.NgayCapNhat = DateTime.Now;

            int res = await _serviceService.UpdateAsync(existingDV);
            Console.WriteLine($"UpdateAsync result: {res}");
            return StatusCode(204, res);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDichVu(Guid id)
        {
            try
            {
                // Xóa dịch vụ theo id
                int result = await _serviceService.DeleteAsync(id);

                if (result > 0)
                {
                    return NoContent(); // Trả về 204 No Content khi xóa thành công
                }
                else
                {
                    return NotFound("Không tìm thấy dịch vụ để xóa.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi xóa dịch vụ: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("import-dichvu")]
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

            var listDV = new List<DichVu>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;
                    for (int row = 2; row <= rowCount; row++)
                    {
                        if (string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Text)) continue; // Dòng trống
                        var dichVu = new DichVuModel
                        {
                            KhoaId = Guid.TryParse(worksheet.Cells[row, 2].Text, out var khoaId) ? khoaId : null,
                            MaDichVu = worksheet.Cells[row, 3].Text,
                            TenDichVu = worksheet.Cells[row, 4].Text,
                            MoTaDichVu = worksheet.Cells[row, 5].Text,
                            DonGia = decimal.TryParse(worksheet.Cells[row, 6].Text, out var donGia) ? donGia : 0
                        };

                        var entity = _mapper.Map<DichVu>(dichVu);
                        listDV.Add(entity);
                    }
                }
            }

            var result = await _serviceService.ImportDataFromExcel(listDV);

            return Ok(new { success = true, message = $"{result} bản ghi được thêm" });
        }

    }
}
