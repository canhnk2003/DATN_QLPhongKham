using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QuanLyPhongKham.Business.Interfaces;
using QuanLyPhongKham.Data.Context;
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

namespace QuanLyPhongKham.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<object> LoginAsync(LoginModel model)
        {
            var user = await _userRepository.FindByNameAsync(model.Username);
            if (user != null && await _userRepository.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userRepository.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                }

                var token = CreateToken(authClaims);
                var refreshToken = GenerateRefreshToken();

                _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);

                await _userRepository.UpdateUserAsync(user);

                return new
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = refreshToken,
                    Expiration = token.ValidTo
                };
            }
            return null;
        }

        public async Task<Response> RegisterAsync(RegisterModel model)
        {
            var userExists = await _userRepository.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                return new Response { Status = "Error", Message = "User already exists!" };
            }

            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userRepository.CreateUserAsync(user, model.Password);
            if (!result)
            {
                return new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." };
            }
            //kiem tra role Customer da co chua
            if (!await _roleManager.RoleExistsAsync(UserRoles.Patient))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Patient));
            }
            await _userRepository.AddToRoleAsync(user, UserRoles.Patient);
            return new Response { Status = "Success", Message = "User created successfully!", Data = user.Id };
        }

        public async Task<Response> RegisterDoctorAsync(RegisterModel model)
        {
            var userExists = await _userRepository.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                return new Response { Status = "Error", Message = "User already exists!" };
            }

            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userRepository.CreateUserAsync(user, model.Password);
            if (!result)
            {
                return new Response{ Status = "Error", Message = "User creation failed! Please check user details and try again." };
            }
            //kiem tra role Doctor da co chua
            if (!await _roleManager.RoleExistsAsync(UserRoles.Doctor))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Doctor));
            }
            await _userRepository.AddToRoleAsync(user, UserRoles.Doctor);
            return new Response { Status = "Success", Message = "User created successfully!", Data = user.Id };
        }

        public async Task<Response> RegisterAdminAsync(RegisterModel model)
        {
            var userExists = await _userRepository.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                return new Response { Status = "Error", Message = "User already exists!" };
            }

            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userRepository.CreateUserAsync(user, model.Password);
            if (!result)
            {
                return new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." };
            }
            //kiem tra role Admin da co chua
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }
            await _userRepository.AddToRoleAsync(user, UserRoles.Admin);
            return new Response { Status = "Success", Message = "User created successfully!" };
        }

        public async Task<Response> RefreshTokenAsync(TokenModel tokenModel)
        {
            if (tokenModel == null)
            {
                return new Response { Status = "Error", Message = "Invalid client request" };
            }

            string? accessToken = tokenModel.AccessToken;
            string? refreshToken = tokenModel.RefreshToken;

            var principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                return new Response{ Status = "Error", Message = "Invalid access token or refresh token" };
            }

            string username = principal.Identity.Name;
            var user = await _userRepository.FindByNameAsync(username);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return new Response { Status = "Error", Message = "Invalid access token or refresh token" };
            }

            var newAccessToken = CreateToken(principal.Claims.ToList());
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userRepository.UpdateUserAsync(user);

            return new Response
            {
                Status = "Success",
                Message = "Token refreshed successfully",
                Data = new
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                    RefreshToken = newRefreshToken
                }
            };
        }

        public async Task<string> Revoke(string username)
        {
            var user = await _userRepository.FindByNameAsync(username);
            if (user == null) return "Invalid user name";

            user.RefreshToken = null;
            await _userRepository.UpdateUserAsync(user);

            return "Success";
        }
        
        public async Task<string> RevokeAll()
        {
            var users = await _userRepository.GetAllUserAsync();
            foreach (var user in users)
            {
                user.RefreshToken = null;
                await _userRepository.UpdateUserAsync(user);
            }

            return "Success";
        }


        private static string GenerateRefreshToken()
        {
            var randomNumber = new Byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private JwtSecurityToken CreateToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                    ValidateLifetime = false
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }

                return principal;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Response> DeleteUser(string userId)
        {
            bool deleted = await _userRepository.DeleteUserAsync(userId);
            if (!deleted)
            {
                throw new ErrorDeleteException();
            }
            return new Response { Status = "Success", Message = "Xóa thành công" };
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            //var res = await _userRepository.FindByIdAsync(userId);
            //if (res == null)
            //{
            //    throw new ErrorNotFoundException();
            //}
            //return res;
            return await _userRepository.FindByIdAsync(userId);
        }

        public async Task<Service> ChangePasswordAsync(string username, string curPassword, string newPassword)
        {
            bool changed = await _userRepository.ChangePasswordAsync(username, curPassword, newPassword);
            if (!changed)
            {
                return new Service {DevMsg = "Error", UserMsg = "Mật khẩu không đúng vui lòng nhập lại!" };
            }
            return new Service { UserMsg = "Đổi mật khẩu thành công!" };
        }

        public async Task<string> FindByUserNameAsync(string userName)
        {
            var user = await _userRepository.FindByNameAsync(userName);
            if (user == null) return "Invalid user name";
            return user.Id;
        }

        public async Task<Response> ResetPasswordAsync(string email)
        {
            var res = await _userRepository.ResetPassWordAsync(email);
            if (!res)
            {
                return new Response { Status = "Error", Message = "Có lỗi xảy ra!" };
            }
            return new Response { Status = "Success", Message = "Reset mật khẩu thành công" };
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUserAsync()
        {
            var res = await _userRepository.GetAllUserAsync();
            return res;
        }

        public async Task<IList<string>> GetUserRole(string userId)
        {
            var user = await _userRepository.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ErrorNotFoundException();
            }

            // Lấy danh sách các vai trò của người dùng
            var roles = await _userRepository.GetRolesAsync(user);
            return roles;

        }

        public async Task<Response> DeleteUserAsync(string userId)
        {
            var res = await _userRepository.DeleteUserAsync(userId);
            if (!res)
            {
                return new Response { Status = "Error", Message = "Có lỗi xảy ra!" };
            }
            return new Response { Status = "Success", Message = "Xóa người dùng thành công" };
        }
    }
}
