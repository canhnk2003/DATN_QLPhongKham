using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Business.Services;
using QuanLyPhongKham.Models.Models;
using System.Security.Claims;

namespace QuanLyPhongKham.WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly IGeminiService _geminiService;
        private readonly IMemoryCache _cache;
        public ChatsController(IGeminiService geminiService, IMemoryCache cache)
        {
            _geminiService = geminiService;
            _cache = cache;
        }
        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            if (string.IsNullOrEmpty(request.Message))
            {
                return BadRequest("Tin nhắn không được để trống!");
            }

            //Lấy userId từ JWT Token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Không xác định được người dùng!");
            }

            // Lấy session từ MemoryCache dựa vào UserId
            var sessionKey = $"ChatSession_{userId}";
            var context = _cache.Get<ChatContext>(sessionKey) ?? new ChatContext();

            // Gửi câu hỏi lên AI với ngữ cảnh
            var response = await _geminiService.GetResponseFromGemini(request.Message, context);

            // Lưu lại session (cập nhật context)
            _cache.Set(sessionKey, context, TimeSpan.FromMinutes(10));

            return Ok(response);
        }
    }
}
