using cv_management.Models;
using CvManagementApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CvManagementApi.Controllers
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == req.Username && u.PasswordHash == req.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Sai username hoặc password" });
            }


            var roles = await _context.UserRoles
                .Where(ur => ur.UserId == user.UserId)
                .Join(_context.Roles,
                      ur => ur.RoleId,
                      r => r.RoleId,
                      (ur, r) => r.RoleName)
                .ToListAsync();



            // Tạo JWT token
            var token = GenerateJwtToken(user, roles);

            return Ok(new
            {
                message = "Đăng nhập thành công",
                token = token,
                user = new
                {
                    user.UserId,
                    user.Username,
                    user.FullName,
                    user.Email ,
                    Roles = roles  
                }
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            try
            {
           
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == req.Username);
                
                if (existingUser != null)
                {
                    return BadRequest(new { message = "Username đã tồn tại" });
                }

              
                var existingEmail = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == req.Email);
                
                if (existingEmail != null)
                {
                    return BadRequest(new { message = "Email đã tồn tại" });
                }

              
                var newUser = new User
                {
                    Username = req.Username,
                    Email = req.Email,
                    PasswordHash = req.Password, 
                    FullName = req.FullName,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                
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

                
                var userRoleAssignment = new UserRole
                {
                    UserId = newUser.UserId,
                    RoleId = userRole.RoleId
                };

                _context.UserRoles.Add(userRoleAssignment);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Đăng ký thành công",
                    user = new
                    {
                        newUser.UserId,
                        newUser.Username,
                        newUser.FullName,
                        newUser.Email,
                        Role = "User"
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        private string GenerateJwtToken(User user, List<string> roles)
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

            // Thêm roles vào claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Token hết hạn sau 1 giờ
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FullName { get; set; } = null!;
    }
}
