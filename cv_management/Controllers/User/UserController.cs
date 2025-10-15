using cv_management.Models;
using cv_management.Controllers.User.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserModel = cv_management.Models.User;

namespace cv_management.Controllers.User
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly CvManagementContext _context;

        public UserController(CvManagementContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy thông tin profile của user hiện tại
        /// </summary>
        /// <returns>Thông tin profile user</returns>
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst("id")!.Value);
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    return NotFound(new { message = "User không tồn tại" });
                }

                var response = new UserProfileResponse
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật thông tin profile của user
        /// </summary>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("id")!.Value);
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    return NotFound(new { message = "User không tồn tại" });
                }

                // Update user information
                user.FullName = request.FullName;
                user.Email = request.Email;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Cập nhật profile thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách CV của user
        /// </summary>
        /// <returns>Danh sách CV</returns>
        [HttpGet("cvs")]
        public async Task<IActionResult> GetUserCVs()
        {
            try
            {
                var userId = int.Parse(User.FindFirst("id")!.Value);
                
                var cvs = await _context.Cvs
                    .Where(cv => cv.UserId == userId)
                    .Select(cv => new
                    {
                        cv.CvId,
                        cv.Title,
                        cv.IsPublic,
                        cv.IsDefault,
                        cv.CreatedAt,
                        cv.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(cvs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }
    }
}
