using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Business.Services;
using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Models;
using QuanLyPhongKham.Data.Repositories;
using QuanLyPhongKham.Data.Interfaces;
using Microsoft.Extensions.Logging;


namespace QuanLyPhongKham.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly IResultService _resultService;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IPatientService _patientService;
        private readonly IAppointmentService _appointmentService;
        private readonly IResultRepository _resultRepository;
        private readonly ILogger<ResultsController> _logger;



        public ResultsController(IResultService resultService, IAuthService authService, IMapper mapper, IPatientService patientService, IAppointmentService appointmentService, ILogger<ResultsController> logger)
        {
            _resultService = resultService;
            _authService = authService;
            _mapper = mapper;
            _patientService = patientService;
            _appointmentService = appointmentService;
            _logger = logger;


        }

        [HttpGet]
        public async Task<ActionResult> getKetQuaKham()
        {
            var ketquas = await _resultService.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<ResultModel>>(ketquas));
        }

        [HttpGet("{KetQuaKhamId}")]
        public async Task<IActionResult> GetResultById(Guid KetQuaKhamId)
        {
            var result = await _resultService.GetByIdAsync(KetQuaKhamId);
            return Ok(_mapper.Map<ResultModel>(result));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] KetQuaKham ketQuaKham)
        {
            var lkId = ketQuaKham.LichKhamId;
            if (lkId == Guid.Empty) // Kiểm tra LichKhamId có hợp lệ không
            {
                return BadRequest("LichKhamId không hợp lệ.");
            }
            int result = await _resultService.AddAsync(ketQuaKham);
            int res = await _appointmentService.Complete((Guid) lkId);
            if (result > 0 && res > 0)
            {
                return StatusCode(201, "Thêm mới kết quả khám thành công.");
            }
            else
            {
                return BadRequest("Thêm mới kết quả khám thất bại.");
            }

        }




        [HttpPut("{ketQuaKhamId}")]
        public async Task<IActionResult> UpdateResult(Guid ketQuaKhamId, [FromBody] ResultModel ketQuaKham)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (ketQuaKhamId != ketQuaKham.KetQuaKhamId)
            {
                return BadRequest("Id không giống!");
            }

            var existingKQ = await _resultService.GetByIdAsync(ketQuaKhamId);
            if (existingKQ == null)
            {
                return BadRequest("Không tìm thấy kết quả");
            }
            existingKQ.LichKhamId = ketQuaKham.LichKhamId;
            existingKQ.ChanDoan = ketQuaKham.ChanDoan;
            existingKQ.ChiDinhThuoc = ketQuaKham.ChiDinhThuoc;
            existingKQ.GhiChu = ketQuaKham.GhiChu;
            existingKQ.NgayCapNhat = DateTime.Now;

            int res = await _resultService.UpdateAsync(existingKQ);
            Console.WriteLine($"UpdateAsync result: {res}");
            return StatusCode(204, res);
        }




        [HttpGet("ketqua/{LichKhamId}")]
        public ActionResult GetKetQuaByLichKhamId(Guid LichKhamId)
        {
            var kq = _resultService.GetKetQuaKhamByLichKhamId(LichKhamId);
            return Ok(kq);
        }

        [HttpGet("doctor/{bacSiId}")]
        public async Task<IActionResult> GetResultsByDoctorId(Guid bacSiId)
        {
            var results = await _resultService.GetAllByDoctorIdAsync(bacSiId);
            if (results == null || !results.Any())
            {
                return NotFound("Không có kết quả khám nào cho bác sĩ này.");
            }
            return Ok(results);
        }

        [HttpGet("ketquakham/{benhNhanId}")]
        public async Task<IActionResult> GetResultsByPatientId(Guid benhNhanId)
        {
            var results = await _resultService.GetAllByPatientIdAsync(benhNhanId);
            if (results == null || !results.Any())
            {
                return NotFound("Không có kết quả khám nào cho bệnh nhân này.");
            }
            return Ok(results);
        }

        [HttpGet("tenbenhnhan/{lichKhamId}")]
        public ActionResult GetBenhNhanName(Guid lichKhamId)
        {
            var benhNhanName = _resultService.GetBenhNhanNameByLichKhamId(lichKhamId);
            if (benhNhanName != null)
            {
                return Ok(benhNhanName);
            }
            else
            {
                return NotFound("Không tìm thấy bệnh nhân.");
            }
        }
    }
}