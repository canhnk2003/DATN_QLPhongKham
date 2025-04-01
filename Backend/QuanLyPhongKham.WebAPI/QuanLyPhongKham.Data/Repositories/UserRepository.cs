using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QuanLyPhongKham.Data.Interfaces;
using QuanLyPhongKham.Models.Entities;
using QuanLyPhongKham.Models.Exceptions;
using QuanLyPhongKham.Models.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApplicationUser> FindByNameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<bool> UpdateUserAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> CreateUserAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result.Succeeded;
        }

        public async Task AddToRoleAsync(ApplicationUser user, string role)
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<bool> RoleExistsAsync(string role)
        {
            return await _roleManager.RoleExistsAsync(role);
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password); 
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUserAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ErrorNotFoundException();
            }
            var result = await _userManager.DeleteAsync(user); // Xóa người dùng
            return result.Succeeded; // Trả về true nếu xóa thành công, false nếu không
        }

        public async Task<bool> ChangePasswordAsync(string username, string curPassword, string newPassword)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                throw new ErrorNotFoundException();
            }
            if (!ValidatePassword(newPassword))
            {
                throw new ErrorChangePassException();
            }
            var result = await _userManager.ChangePasswordAsync(user, curPassword, newPassword);
            return result.Succeeded;
        }

        public bool ValidatePassword(string password)
        {
            bool hasUpperChar = false;
            bool hasLowerChar = false;
            bool hasDigit = false;
            bool hasSpecialChar = false;
            string specialChars = @"!@#$%^&*()_+[]{}|;:',.<>?/`~-=\\";

            foreach (var c in password)
            {
                if (char.IsUpper(c)) hasUpperChar = true;
                if (char.IsLower(c)) hasLowerChar = true;
                if (char.IsDigit(c)) hasDigit = true;
                if (specialChars.Contains(c)) hasSpecialChar = true;
            }

            return hasUpperChar && hasLowerChar && hasDigit && hasSpecialChar;
        }

        public async Task<bool> ResetPassWordAsync(string email)
        {
            var user = await _userManager.FindByIdAsync(email);
            if (user == null)
            {
                throw new ErrorNotFoundException();
            }
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, "Abc@123");
            if (result.Succeeded)
            {
                return true;
            }
            return false;
        }
    }
}
