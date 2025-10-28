using cv_management.Controllers.Jobs.DTOs;
using cv_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace cv_management.Controllers.Jobs
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly CvManagementContext _context;

        public JobsController(CvManagementContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tất cả Jobs (có phân trang)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllJobs(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            [FromQuery] string? location = null,
            [FromQuery] string? jobType = null)
        {
            try
            {
                var query = _context.Jobs
                    .Include(j => j.Company)
                    .AsQueryable();

                // Lọc theo status
                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(j => j.Status == status);
                }

                // Lọc theo location
                if (!string.IsNullOrEmpty(location))
                {
                    query = query.Where(j => j.Location != null && j.Location.Contains(location));
                }

                // Lọc theo jobType
                if (!string.IsNullOrEmpty(jobType))
                {
                    query = query.Where(j => j.JobType == jobType);
                }

                var totalCount = await query.CountAsync();

                var jobs = await query
                    .OrderByDescending(j => j.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(j => new JobResponse
                    {
                        JobId = j.JobId,
                        CompanyId = j.CompanyId,
                        CompanyName = j.Company.Name,
                        Title = j.Title,
                        Description = j.Description,
                        Requirements = j.Requirements,
                        SalaryRange = j.SalaryRange,
                        Location = j.Location,
                        JobType = j.JobType,
                        Status = j.Status,
                        CreatedAt = j.CreatedAt,
                        UpdatedAt = j.UpdatedAt
                    })
                    .ToListAsync();

                var response = new JobListResponse
                {
                    Jobs = jobs,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết 1 Job theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobById(int id)
        {
            try
            {
                var job = await _context.Jobs
                    .Include(j => j.Company)
                    .FirstOrDefaultAsync(j => j.JobId == id);

                if (job == null)
                {
                    return NotFound(new { message = "Không tìm thấy Job" });
                }

                var response = new JobResponse
                {
                    JobId = job.JobId,
                    CompanyId = job.CompanyId,
                    CompanyName = job.Company.Name,
                    Title = job.Title,
                    Description = job.Description,
                    Requirements = job.Requirements,
                    SalaryRange = job.SalaryRange,
                    Location = job.Location,
                    JobType = job.JobType,
                    Status = job.Status,
                    CreatedAt = job.CreatedAt,
                    UpdatedAt = job.UpdatedAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách Jobs của 1 Company
        /// </summary>
        [HttpGet("company/{companyId}")]
        public async Task<IActionResult> GetJobsByCompany(int companyId)
        {
            try
            {
                var jobs = await _context.Jobs
                    .Include(j => j.Company)
                    .Where(j => j.CompanyId == companyId)
                    .OrderByDescending(j => j.CreatedAt)
                    .Select(j => new JobResponse
                    {
                        JobId = j.JobId,
                        CompanyId = j.CompanyId,
                        CompanyName = j.Company.Name,
                        Title = j.Title,
                        Description = j.Description,
                        Requirements = j.Requirements,
                        SalaryRange = j.SalaryRange,
                        Location = j.Location,
                        JobType = j.JobType,
                        Status = j.Status,
                        CreatedAt = j.CreatedAt,
                        UpdatedAt = j.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(jobs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        /// <summary>
        /// Company tạo Job mới (Yêu cầu đăng nhập)
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateJob([FromBody] CreateJobRequest request)
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

                // Kiểm tra user có company không
                var company = await _context.Companies
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (company == null)
                {
                    return BadRequest(new { message = "Bạn cần tạo Company profile trước khi đăng Job" });
                }

                // Validate input
                if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description))
                {
                    return BadRequest(new { message = "Title và Description không được để trống" });
                }

                // Tạo Job mới
                var newJob = new Job
                {
                    CompanyId = company.CompanyId,
                    Title = request.Title,
                    Description = request.Description,
                    Requirements = request.Requirements,
                    SalaryRange = request.SalaryRange,
                    Location = request.Location,
                    JobType = request.JobType,
                    Status = request.Status ?? "Active",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Jobs.Add(newJob);
                await _context.SaveChangesAsync();

                // Trả về thông tin Job vừa tạo
                var response = new JobResponse
                {
                    JobId = newJob.JobId,
                    CompanyId = newJob.CompanyId,
                    CompanyName = company.Name,
                    Title = newJob.Title,
                    Description = newJob.Description,
                    Requirements = newJob.Requirements,
                    SalaryRange = newJob.SalaryRange,
                    Location = newJob.Location,
                    JobType = newJob.JobType,
                    Status = newJob.Status,
                    CreatedAt = newJob.CreatedAt,
                    UpdatedAt = newJob.UpdatedAt
                };

                return CreatedAtAction(nameof(GetJobById), new { id = newJob.JobId }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật thông tin Job (Chỉ Company owner)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] UpdateJobRequest request)
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

                // Tìm Job
                var job = await _context.Jobs
                    .Include(j => j.Company)
                    .FirstOrDefaultAsync(j => j.JobId == id);

                if (job == null)
                {
                    return NotFound(new { message = "Không tìm thấy Job" });
                }

                // Kiểm tra quyền sở hữu (chỉ company owner mới được update)
                if (job.Company.UserId != userId)
                {
                    return Forbid();
                }

                // Update các trường nếu có giá trị mới
                if (!string.IsNullOrWhiteSpace(request.Title))
                    job.Title = request.Title;

                if (!string.IsNullOrWhiteSpace(request.Description))
                    job.Description = request.Description;

                if (request.Requirements != null)
                    job.Requirements = request.Requirements;

                if (request.SalaryRange != null)
                    job.SalaryRange = request.SalaryRange;

                if (request.Location != null)
                    job.Location = request.Location;

                if (request.JobType != null)
                    job.JobType = request.JobType;

                if (request.Status != null)
                    job.Status = request.Status;

                job.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var response = new JobResponse
                {
                    JobId = job.JobId,
                    CompanyId = job.CompanyId,
                    CompanyName = job.Company.Name,
                    Title = job.Title,
                    Description = job.Description,
                    Requirements = job.Requirements,
                    SalaryRange = job.SalaryRange,
                    Location = job.Location,
                    JobType = job.JobType,
                    Status = job.Status,
                    CreatedAt = job.CreatedAt,
                    UpdatedAt = job.UpdatedAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        /// <summary>
        /// Xóa Job (Chỉ Company owner)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteJob(int id)
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

                // Tìm Job
                var job = await _context.Jobs
                    .Include(j => j.Company)
                    .FirstOrDefaultAsync(j => j.JobId == id);

                if (job == null)
                {
                    return NotFound(new { message = "Không tìm thấy Job" });
                }

                // Kiểm tra quyền sở hữu
                if (job.Company.UserId != userId)
                {
                    return Forbid();
                }

                _context.Jobs.Remove(job);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Xóa Job thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }
    }
}

