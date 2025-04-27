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
using System.Numerics;


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
        private readonly IEmailService _emailService;
        private readonly IDoctorService _doctorService;
        private readonly IServiceService _serviceService;
        private readonly ILogger<ResultsController> _logger;

        public ResultsController(IResultService resultService, IAuthService authService, IMapper mapper, IPatientService patientService,
            IAppointmentService appointmentService, ILogger<ResultsController> logger, IEmailService emailService, IDoctorService doctorService,
            IServiceService serviceService)
        {
            _resultService = resultService;
            _authService = authService;
            _mapper = mapper;
            _patientService = patientService;
            _appointmentService = appointmentService;
            _logger = logger;
            _emailService = emailService;
            _doctorService = doctorService;
            _serviceService = serviceService;
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
            if (lkId == Guid.Empty)
            {
                return BadRequest("LichKhamId không hợp lệ.");
            }

            // 1. Check xem kết quả khám đã tồn tại chưa
            //var existingResult = await _resultService.GetByIdAsync(lkId.Value);
            //if (existingResult != null)
            //{
            //    return BadRequest("Kết quả khám đã tồn tại cho lịch khám này.");
            //}

            // 2. Thêm mới kết quả khám
            int result = await _resultService.AddAsync(ketQuaKham);
            int res = await _appointmentService.Complete(lkId.Value);

            if (result > 0 && res > 0)
            {
                // 3. Lấy đầy đủ thông tin để gửi email
                var lk = await _appointmentService.GetByIdAsync(lkId.Value);
                var kq = _resultService.GetKetQuaKhamByLichKhamId(lkId.Value);

                if (lk != null && kq != null)
                {
                    var bn = await _patientService.GetByIdAsync(lk.BenhNhanId);
                    var bs = await _doctorService.GetByIdAsync(lk.BacSiId);
                    var dv = await _serviceService.GetByIdAsync(lk.DichVuId.Value);

                    // Validate chiDinhThuoc và ghiChu
                    string[] chiDinhThuocArr = kq.ChiDinhThuoc?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
                    string[] ghiChuArr = kq.GhiChu?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];

                    if (chiDinhThuocArr.Length != ghiChuArr.Length)
                    {
                        return BadRequest("Số lượng chỉ định thuốc và ghi chú không khớp nhau.");
                    }

                    // 4. Gửi email
                    try
                    {
                        var body = BuildEmailBody(bn, bs, dv, lk, kq, chiDinhThuocArr, ghiChuArr);
                        await _emailService.SendEmailAsync(bn.Email, "Kết quả khám bệnh", body);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Gửi email thất bại.");
                        return StatusCode(500, "Thêm mới kết quả thành công nhưng gửi email thất bại.");
                    }

                    return CreatedAtAction(nameof(Post), new { id = ketQuaKham.KetQuaKhamId }, new
                    {
                        statusCode = 201,
                        response = res,
                        message = "Thêm mới kết quả thành công và đã gửi email xác nhận."
                    });
                }

                return StatusCode(500, "Không tìm thấy dữ liệu để gửi email.");
            }

            return BadRequest("Thêm mới kết quả khám thất bại.");
        }

        private string BuildEmailBody(BenhNhan bn, BacSi bs, DichVu dv, LichKham lk, KetQuaKham kq, string[] chiDinhThuocArr, string[] ghiChuArr)
        {
            string tableContent = "";
            for (int i = 0; i < chiDinhThuocArr.Length; i++)
            {
                tableContent += $@"
            <tr>
                <td>{i + 1}</td>
                <td>{chiDinhThuocArr[i].Trim()}</td>
                <td>{ghiChuArr[i].Trim()}</td>
            </tr>";
            }

            var body = $@"
    <html lang='vi'>
    <head>
        <meta charset='UTF-8'>
        <title>Kết quả khám bệnh</title>
        <style>
            body {{ background-color: #f4f4f9; color: #333; padding: 20px; }}
            .container {{ width: 100%; max-width: 600px; margin: 0 auto; background-color: #fff; padding: 20px; border-radius: 8px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1); }}
            .header {{ background-color: #007bff; color: #fff; text-align: center; padding: 15px; border-radius: 8px 8px 0 0; }}
            .content {{ margin-top: 20px; }}
            .footer {{ margin-top: 30px; text-align: center; font-size: 14px; background-color: #f9f9f9; padding: 15px; border-radius: 0 0 8px 8px; }}
            table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
            th, td {{ padding: 10px; border: 1px solid #ddd; text-align: left; }}
            th {{ background-color: #007bff; color: #fff; }}
            tr:nth-child(even) {{ background-color: #f9f9f9; }}
            .no-border-table th, .no-border-table td {{border: none;}}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'>
                <h2>Kết quả khám bệnh</h2>
            </div>
            <div class='content'>
                <p>Xin chào {bn.HoTen},</p>
                <p>Cảm ơn bạn đã khám bệnh tại Phòng khám ABC. Dưới đây là kết quả khám của bạn:</p>

                <h3>Thông tin lịch khám</h3>                            
                <table>
                    <tr><td><strong>Ngày khám:</strong></td><td>{lk.NgayKham:dd/MM/yyyy}</td></tr>
                    <tr><td><strong>Giờ khám:</strong></td><td>{lk.GioKham}</td></tr>
                    <tr><td><strong>Bác sĩ:</strong></td><td>{bs.HoTen}</td></tr>
                    <tr><td><strong>Bệnh nhân:</strong></td><td>{bn.HoTen}</td></tr>
                    <tr><td><strong>Dịch vụ:</strong></td><td>{dv.TenDichVu}</td></tr>
                </table>

                <h3>Nội dung khám</h3>
                    <h4>1. Tiền sử bệnh lý:</h4>
                    <p>{bn.TienSuBenhLy}</p>
                    <h4>2. Chẩn đoán:</h4>
                    <p>{kq.ChanDoan}</p>

                <h3>Đơn thuốc</h3>
                <table>
                    <thead>
                        <tr>
                            <th>STT</th>
                            <th>Chỉ định thuốc</th>
                            <th>Ghi chú</th>
                        </tr>
                    </thead>
                    <tbody>
                        {tableContent}
                    </tbody>
                </table>

                <p>Cảm ơn bạn đã tin tưởng và đồng hành cùng Phòng khám ABC. Hẹn gặp lại bạn!</p>
            </div>
            <div class='footer'>
                <p>Phòng Khám ABC</p>
                <p><a href='http://127.0.0.1:5500/User/index.html'>Truy cập trang web của chúng tôi</a></p>
            </div>
        </div>
    </body>
    </html>";

            return body;
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