using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Business.Services;
using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Models;

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
	}
}
