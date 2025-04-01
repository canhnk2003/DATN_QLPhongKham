using Microsoft.AspNetCore.Identity;
using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Data.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<ApplicationUser>> GetAllUserAsync();
        Task<ApplicationUser> FindByNameAsync(string username);
        Task<bool> UpdateUserAsync(ApplicationUser user);
        Task<bool> CreateUserAsync(ApplicationUser user, string password);
        Task AddToRoleAsync(ApplicationUser user, string role);
        Task<bool> RoleExistsAsync(string role);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
        Task<ApplicationUser> FindByIdAsync(string userId);
        Task<bool> DeleteUserAsync(string userId);
        Task<bool> ChangePasswordAsync(string username, string curPassword, string newPassword);
        Task<bool> ResetPassWordAsync(string email);
    }
}
