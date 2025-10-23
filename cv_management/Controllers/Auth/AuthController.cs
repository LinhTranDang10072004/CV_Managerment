using cv_management.Models;
using cv_management.Controllers.Auth.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserModel = cv_management.Models.User;

namespace cv_management.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly CvManagementContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(CvManagementContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Đăng nhập user
        /// </summary>
        /// <param name="request">Thông tin đăng nhập</param>
        /// <returns>JWT token và thông tin user</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new { message = "Username và Password không được để trống" });
                }

                // Find user by username
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == request.Username && u.PasswordHash == request.Password);

                if (user == null)
                {
                    return Unauthorized(new { message = "Sai username hoặc password" });
                }

                // Get user roles
                var roles = await _context.UserRoles
                    .Where(ur => ur.UserId == user.UserId)
                    .Join(_context.Roles,
                          ur => ur.RoleId,
                          r => r.RoleId,
                          (ur, r) => r.RoleName)
                    .ToListAsync();

                // Generate JWT token
                var token = GenerateJwtToken(user, roles);

                var response = new AuthResponse
                {
                    Message = "Đăng nhập thành công",
                    Token = token,
                    User = new UserInfo
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        FullName = user.FullName,
                        Email = user.Email,
                        Roles = roles
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        /// <summary>
        /// Đăng ký user mới
        /// </summary>
        /// <param name="request">Thông tin đăng ký</param>
        /// <returns>JWT token và thông tin user</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(request.Username) || 
                    string.IsNullOrWhiteSpace(request.Email) || 
                    string.IsNullOrWhiteSpace(request.Password) ||
                    string.IsNullOrWhiteSpace(request.FullName))
                {
                    return BadRequest(new { message = "Tất cả các trường đều bắt buộc" });
                }

                // Check if username already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == request.Username);
                
                if (existingUser != null)
                {
                    return BadRequest(new { message = "Username đã tồn tại" });
                }

                // Check if email already exists
                var existingEmail = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email);
                
                if (existingEmail != null)
                {
                    return BadRequest(new { message = "Email đã tồn tại" });
                }

                // Create new user
                var newUser = new UserModel
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = request.Password, // Lưu password trực tiếp (chưa mã hóa)
                    FullName = request.FullName,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // Find or create "User" role
                var userRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.RoleName == "User");

                if (userRole == null)
                {
                    userRole = new Role
                    {
                        RoleName = "User",
                        Description = "Người dùng thông thường"
                    };
                    _context.Roles.Add(userRole);
                    await _context.SaveChangesAsync();
                }

                // Assign "User" role to new user
                var userRoleAssignment = new UserRole
                {
                    UserId = newUser.UserId,
                    RoleId = userRole.RoleId
                };

                _context.UserRoles.Add(userRoleAssignment);
                await _context.SaveChangesAsync();

                // Generate JWT token
                var token = GenerateJwtToken(newUser, new List<string> { "User" });

                var response = new AuthResponse
                {
                    Message = "Đăng ký thành công",
                    Token = token,
                    User = new UserInfo
                    {
                        UserId = newUser.UserId,
                        Username = newUser.Username,
                        FullName = newUser.FullName,
                        Email = newUser.Email,
                        Roles = new List<string> { "User" }
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        /// <summary>
        /// Test JWT token (để debug)
        /// </summary>
        /// <returns>Thông tin từ token</returns>
        [HttpPost("test-token")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult TestToken()
        {
            var userId = User.FindFirst("id")?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            return Ok(new
            {
                message = "Token hợp lệ",
                userId = userId,
                username = username,
                email = email,
                roles = roles
            });
        }

        private string GenerateJwtToken(UserModel user, List<string> roles)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? "default_key"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("id", user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("FullName", user.FullName)
            };

            // Add roles to claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Token expires after 1 hour
                signingCredentials: credentials                                         
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
