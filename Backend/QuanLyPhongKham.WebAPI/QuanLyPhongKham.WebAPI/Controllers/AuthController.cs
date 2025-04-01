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
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IPatientService _petientService;
        private readonly IDoctorService _doctorService;

        public AuthController(IAuthService authService, IPatientService petientService, IDoctorService doctorService)
        {
            _authService = authService;
            _petientService = petientService;
            _doctorService = doctorService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            var res = await _authService.GetAllUserAsync();
            var ds = new List<UserModel>();
            foreach (var user in res)
            {
                var roles = await _authService.GetUserRole(user.Id);
                var item = new UserModel();
                item.Id = user.Id;
                item.UserName = user.UserName;
                item.Email = user.Email;
                item.Roles = roles;
                ds.Add(item);
            }
            return Ok(ds);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (model == null)
            {
                return BadRequest(new { message = "Invalid client request" });
            }

            var result = await _authService.RegisterAsync(model);
            if (result.Status == "Success")
            {
                BenhNhan bn = new BenhNhan();
                bn.Email = model.Email;
                bn.HoTen = model.Username;
                bn.UserId = result.Data.ToString();
                await _petientService.AddAsync(bn);
                return Ok(result); // Trả về thông báo thành công
            }

            return BadRequest(result); // Trả về thông báo lỗi
        }

        [HttpPost]
        [Route("register-doctor")]
        public async Task<IActionResult> RegisterDoctor([FromBody] RegisterModel model)
        {
            if (model == null)
            {
                return BadRequest(new { message = "Invalid client request" });
            }

            var result = await _authService.RegisterDoctorAsync(model);
            if (result.Status == "Success")
            {
                //BenhNhan bn = new BenhNhan();
                //bn.Email = model.Email;
                //bn.HoTen = model.Username;
                //bn.UserId = result.Data.ToString();
                //await _petientService.AddAsync(bn);
                BacSi bs = new BacSi();
                bs.Email = model.Email;
                bs.HoTen = model.Username;
                bs.UserId = result.Data.ToString();
                await _doctorService.AddAsync(bs);
                return Ok(result); // Trả về thông báo thành công
            }

            return BadRequest(result); // Trả về thông báo lỗi
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            if (model == null)
            {
                return BadRequest(new { message = "Invalid client request" });
            }

            var result = await _authService.RegisterAdminAsync(model);
            if (result.Status == "Success")
            {
                return Ok(result); // Trả về thông báo thành công
            }

            return BadRequest(result); // Trả về thông báo lỗi
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (model == null)
            {
                return BadRequest(new { message = "Invalid client request" });
            }

            var result = await _authService.LoginAsync(model);
            if (result is not null && result is not string)
            {
                return Ok(result); // Trả về token và refresh token
            }

            return BadRequest(new { message = result });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var result = await _authService.ChangePasswordAsync(model.username, model.currentPassword, model.newPassword);
            if (!String.IsNullOrEmpty(result.DevMsg))
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] string email)
        {
            var res = await _authService.ResetPasswordAsync(email);
            if (res.Status == "Error")
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenModel tokenModel)
        {
            if (tokenModel == null)
            {
                return BadRequest(new { message = "Invalid client request" });
            }

            var result = await _authService.RefreshTokenAsync(tokenModel);
            if (result.Status == "Success")
            {
                return Ok(result); // Trả về token mới và refresh token mới
            }

            return BadRequest(new { message = result });
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> FindByUserName(string username)
        {
            if (String.IsNullOrEmpty(username))
            {
                return BadRequest(new { message = "Không được để rỗng" });
            }
            var result = await _authService.FindByUserNameAsync(username);
            if (!String.IsNullOrEmpty(result))
            {
                return Ok(result);
            }
            return BadRequest(new { message = "Không tìm thấy" });
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var res = await _authService.DeleteUserAsync(userId);
            if (res.Status == "Error")
            {
                return BadRequest(res);
            }
            return Ok(res);
        }
    }
}
