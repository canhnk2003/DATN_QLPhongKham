using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Business.Interfaces
{
    public interface IAuthService
    {
        Task<object> LoginAsync(LoginModel model);
        Task<Response> RegisterAsync(RegisterModel model);
        Task<Response> RegisterDoctorAsync(RegisterModel model);
        Task<Response> RegisterAdminAsync(RegisterModel model);
        Task<Response> RefreshTokenAsync(TokenModel tokenModel);
        Task<string> Revoke(string username);
        Task<string> RevokeAll();
        Task<ApplicationUser> FindByIdAsync(string userId);
        Task<Response> DeleteUser(string userId);
        Task<Service> ChangePasswordAsync(string username, string curPassword, string newPassword);
        Task<string> FindByUserNameAsync(string userName);
        Task<Response> ResetPasswordAsync(string email);
        Task<IEnumerable<ApplicationUser>> GetAllUserAsync();
        Task<IList<string>> GetUserRole(string userId);
        Task<Response> DeleteUserAsync(string userId); 
    }
}
