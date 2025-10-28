using cv_management.Controllers.Companies.DTOs;
using cv_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cv_management.Controllers.Companies
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly CvManagementContext _context;

        public CompanyController(CvManagementContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tất cả Companies
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllCompanies()
        {
            try
            {
                var companies = await _context.Companies
                    .Include(c => c.User)
                    .Select(c => new CompanyResponse
                    {
                        CompanyId = c.CompanyId,
                        UserId = c.UserId,
                        Name = c.Name,
                        Address = c.Address,
                        Description = c.Description,
                        Website = c.Website,
                        CreatedAt = c.CreatedAt
                    })
                    .ToListAsync();

                return Ok(companies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        /// <summary>
        /// Lấy thông tin Company theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyById(int id)
        {
            try
            {
                var company = await _context.Companies
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.CompanyId == id);

                if (company == null)
                {
                    return NotFound(new { message = "Không tìm thấy Company" });
                }

                var response = new CompanyResponse
                {
                    CompanyId = company.CompanyId,
                    UserId = company.UserId,
                    Name = company.Name,
                    Address = company.Address,
                    Description = company.Description,
                    Website = company.Website,
                    CreatedAt = company.CreatedAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        /// <summary>
        /// Lấy Company profile của user đang đăng nhập
        /// </summary>
        [HttpGet("my-company")]
        [Authorize]
        public async Task<IActionResult> GetMyCompany()
        {
            try
            {
                var userIdClaim = User.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { message = "Không tìm thấy thông tin user trong token" });
                }

                int userId = int.Parse(userIdClaim);

                var company = await _context.Companies
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (company == null)
                {
                    return NotFound(new { message = "Bạn chưa có Company profile" });
                }

                var response = new CompanyResponse
                {
                    CompanyId = company.CompanyId,
                    UserId = company.UserId,
                    Name = company.Name,
                    Address = company.Address,
                    Description = company.Description,
                    Website = company.Website,
                    CreatedAt = company.CreatedAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        /// <summary>
        /// Tạo Company profile (User phải đăng nhập)
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyRequest request)
        {
            try
            {
                // Lấy UserId từ JWT token
                var userIdClaim = User.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { message = "Không tìm thấy thông tin user trong token" });
                }

                int userId = int.Parse(userIdClaim);

                // Kiểm tra user đã có company chưa (1 user chỉ có 1 company)
                var existingCompany = await _context.Companies
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (existingCompany != null)
                {
                    return BadRequest(new { message = "Bạn đã có Company profile rồi" });
                }

                // Validate input
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return BadRequest(new { message = "Tên công ty không được để trống" });
                }

                // Tạo Company mới
                var newCompany = new Company
                {
                    UserId = userId,
                    Name = request.Name,
                    Address = request.Address,
                    Description = request.Description,
                    Website = request.Website,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Companies.Add(newCompany);
                await _context.SaveChangesAsync();

                var response = new CompanyResponse
                {
                    CompanyId = newCompany.CompanyId,
                    UserId = newCompany.UserId,
                    Name = newCompany.Name,
                    Address = newCompany.Address,
                    Description = newCompany.Description,
                    Website = newCompany.Website,
                    CreatedAt = newCompany.CreatedAt
                };

                return CreatedAtAction(nameof(GetCompanyById), new { id = newCompany.CompanyId }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật Company profile
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] UpdateCompanyRequest request)
        {
            try
            {
                // Lấy UserId từ JWT token
                var userIdClaim = User.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { message = "Không tìm thấy thông tin user trong token" });
                }

                int userId = int.Parse(userIdClaim);

                // Tìm Company
                var company = await _context.Companies
                    .FirstOrDefaultAsync(c => c.CompanyId == id);

                if (company == null)
                {
                    return NotFound(new { message = "Không tìm thấy Company" });
                }

                // Kiểm tra quyền sở hữu
                if (company.UserId != userId)
                {
                    return Forbid();
                }

                // Update các trường
                if (!string.IsNullOrWhiteSpace(request.Name))
                    company.Name = request.Name;

                if (request.Address != null)
                    company.Address = request.Address;

                if (request.Description != null)
                    company.Description = request.Description;

                if (request.Website != null)
                    company.Website = request.Website;

                await _context.SaveChangesAsync();

                var response = new CompanyResponse
                {
                    CompanyId = company.CompanyId,
                    UserId = company.UserId,
                    Name = company.Name,
                    Address = company.Address,
                    Description = company.Description,
                    Website = company.Website,
                    CreatedAt = company.CreatedAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        /// <summary>
        /// Xóa Company profile
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            try
            {
                // Lấy UserId từ JWT token
                var userIdClaim = User.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { message = "Không tìm thấy thông tin user trong token" });
                }

                int userId = int.Parse(userIdClaim);

                // Tìm Company
                var company = await _context.Companies
                    .FirstOrDefaultAsync(c => c.CompanyId == id);

                if (company == null)
                {
                    return NotFound(new { message = "Không tìm thấy Company" });
                }

                // Kiểm tra quyền sở hữu
                if (company.UserId != userId)
                {
                    return Forbid();
                }

                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Xóa Company thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }
    }
}

