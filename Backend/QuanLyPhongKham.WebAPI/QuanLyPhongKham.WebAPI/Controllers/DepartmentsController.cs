using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Business.Interfaces;

namespace QuanLyPhongKham.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentsService;

        public DepartmentsController(IDepartmentService departmentsService)
        {
            _departmentsService = departmentsService;
        }

        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var departments = await _departmentsService.GetAllAsync();
            return Ok(departments);
        }
    }
}
