using cv_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cv_management.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController :ControllerBase
    {
        private readonly CvManagementContext _context;

        public UserController(CvManagementContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetProfile()
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(new
            {
                user.UserId,
                user.FullName,
                user.Email,
                user.CreatedAt,
                user.UpdatedAt,
            });
        }

        [HttpPut("update")]
        public IActionResult UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null) return NotFound();

            user.FullName = request.FullName;
            user.Email = request.Email;
            user.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            return Ok(new { message = "Profile updated successfully" });
        }



    }

    public class UpdateProfileRequest
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
