using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Models;

namespace QuanLyPhongKham.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentsService;
        private readonly IMapper _mapper;

        public DepartmentsController(IDepartmentService departmentsService, IMapper mapper)
        {
            _departmentsService = departmentsService;
            _mapper = mapper;
        }

        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var departments = await _departmentsService.GetAllAsync();
            return Ok(departments);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] KhoaModel khoa)
        {
            int res = await _departmentsService.AddAsync(_mapper.Map<Khoa>(khoa));
            return StatusCode(201, res);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{khoaId}")]
        public async Task<IActionResult> Put([FromBody] KhoaModel khoa, Guid khoaId)
        {
            int res = await _departmentsService.UpdateAsync(_mapper.Map<Khoa>(khoa));
            return StatusCode(201, res);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{khoaId}")]
        public async Task<IActionResult> Delete(Guid khoaId)
        {
            int res = await _departmentsService.DeleteAsync(khoaId);
            return StatusCode(201, res);
        }
    }
}
