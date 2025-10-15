using cv_management.Models;
using cv_management.Controllers.CV.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserModel = cv_management.Models.User;

namespace cv_management.Controllers.CV
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CVController : ControllerBase
    {
        private readonly CvManagementContext _context;

        public CVController(CvManagementContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách CV của user hiện tại
        /// </summary>
        /// <returns>Danh sách CV</returns>
        [HttpGet]
        public async Task<IActionResult> GetUserCVs()
        {
            try
            {
                var userId = int.Parse(User.FindFirst("id")!.Value);
                
                var cvs = await _context.Cvs
                    .Where(cv => cv.UserId == userId)
                    .Select(cv => new CVListResponse
                    {
                        CvId = cv.CvId,
                        Title = cv.Title,
                        IsPublic = cv.IsPublic,
                        IsDefault = cv.IsDefault,
                        CreatedAt = cv.CreatedAt,
                        UpdatedAt = cv.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(cvs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message, inner = ex.InnerException?.Message });
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết của 1 CV
        /// </summary>
        /// <param name="cvId">ID của CV</param>
        /// <returns>Thông tin chi tiết CV</returns>
        [HttpGet("{cvId}")]
        public async Task<IActionResult> GetCVDetails(int cvId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("id")!.Value);
                
                var cv = await _context.Cvs
                    .Include(cv => cv.PersonalInfos)
                    .Include(cv => cv.WorkExperiences)
                    .Include(cv => cv.Educations)
                    .Include(cv => cv.Skills)
                    .Include(cv => cv.Projects)
                    .Include(cv => cv.Certifications)
                    .Include(cv => cv.Languages)
                    .FirstOrDefaultAsync(cv => cv.CvId == cvId && cv.UserId == userId);

                if (cv == null)
                {
                    return NotFound(new { message = "CV không tồn tại hoặc không thuộc về user này" });
                }

                var response = new CVDetailsResponse
                {
                    Cv = new CVInfo
                    {
                        CvId = cv.CvId,
                        Title = cv.Title,
                        IsPublic = cv.IsPublic,
                        IsDefault = cv.IsDefault,
                        CreatedAt = cv.CreatedAt,
                        UpdatedAt = cv.UpdatedAt
                    },
                    PersonalInfo = cv.PersonalInfos.Select(pi => new PersonalInfoResponse
                    {
                        PersonalInfoId = pi.PersonalInfoId,
                        FullName = pi.FullName,
                        DateOfBirth = pi.DateOfBirth,
                        Phone = pi.Phone,
                        Email = pi.Email,
                        Address = pi.Address,
                        Summary = pi.Summary
                    }).FirstOrDefault(),
                    WorkExperiences = cv.WorkExperiences.Select(we => new WorkExperienceResponse
                    {
                        ExperienceId = we.ExperienceId,
                        CompanyName = we.CompanyName,
                        Position = we.Position,
                        StartDate = we.StartDate,
                        EndDate = we.EndDate,
                        Description = we.Description
                    }).ToList(),
                    Educations = cv.Educations.Select(ed => new EducationResponse
                    {
                        EducationId = ed.EducationId,
                        SchoolName = ed.SchoolName,
                        Degree = ed.Degree,
                        Major = ed.Major,
                        StartDate = ed.StartDate,
                        EndDate = ed.EndDate,
                        Description = ed.Description
                    }).ToList(),
                    Skills = cv.Skills.Select(s => new SkillResponse
                    {
                        SkillId = s.SkillId,
                        SkillName = s.SkillName,
                        Proficiency = s.Proficiency
                    }).ToList(),
                    Projects = cv.Projects.Select(p => new ProjectResponse
                    {
                        ProjectId = p.ProjectId,
                        Name = p.Name,
                        Description = p.Description,
                        Technologies = p.Technologies,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate
                    }).ToList(),
                    Certifications = cv.Certifications.Select(c => new CertificationResponse
                    {
                        CertificationId = c.CertificationId,
                        Name = c.Name,
                        Issuer = c.Issuer,
                        IssueDate = c.IssueDate,
                        ExpiryDate = c.ExpiryDate
                    }).ToList(),
                    Languages = cv.Languages.Select(l => new LanguageResponse
                    {
                        LanguageId = l.LanguageId,
                        LanguageName = l.LanguageName,
                        Proficiency = l.Proficiency
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message, inner = ex.InnerException?.Message });
            }
        }

        /// <summary>
        /// Tạo CV mới
        /// </summary>
        /// <param name="request">Thông tin CV</param>
        /// <returns>Kết quả tạo CV</returns>
        [HttpPost]
        public async Task<IActionResult> CreateCV([FromBody] CreateCVRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("id")!.Value);

                // Create new CV
                var newCV = new Cv
                {
                    UserId = userId,
                    Title = request.Title,
                    IsPublic = request.IsPublic ?? false,
                    IsDefault = request.IsDefault ?? false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Cvs.Add(newCV);
                await _context.SaveChangesAsync();

                // Create personal info if provided
                if (request.PersonalInfo != null)
                {
                    var personalInfo = new PersonalInfo
                    {
                        CvId = newCV.CvId,
                        FullName = request.PersonalInfo.FullName,
                        DateOfBirth = request.PersonalInfo.DateOfBirth,
                        Phone = request.PersonalInfo.Phone,
                        Email = request.PersonalInfo.Email,
                        Address = request.PersonalInfo.Address,
                        Summary = request.PersonalInfo.Summary
                    };
                    _context.PersonalInfos.Add(personalInfo);
                }

                // Create work experiences
                if (request.WorkExperiences != null)
                {
                    foreach (var we in request.WorkExperiences)
                    {
                        var workExp = new WorkExperience
                        {
                            CvId = newCV.CvId,
                            CompanyName = we.CompanyName,
                            Position = we.Position,
                            StartDate = we.StartDate,
                            EndDate = we.EndDate,
                            Description = we.Description
                        };
                        _context.WorkExperiences.Add(workExp);
                    }
                }

                // Create educations
                if (request.Educations != null)
                {
                    foreach (var ed in request.Educations)
                    {
                        var education = new Education
                        {
                            CvId = newCV.CvId,
                            SchoolName = ed.SchoolName,
                            Degree = ed.Degree,
                            Major = ed.Major,
                            StartDate = ed.StartDate,
                            EndDate = ed.EndDate,
                            Description = ed.Description
                        };
                        _context.Educations.Add(education);
                    }
                }

                // Create skills
                if (request.Skills != null)
                {
                    foreach (var skill in request.Skills)
                    {
                        var newSkill = new Skill
                        {
                            CvId = newCV.CvId,
                            SkillName = skill.SkillName,
                            Proficiency = skill.Proficiency
                        };
                        _context.Skills.Add(newSkill);
                    }
                }

                // Create projects
                if (request.Projects != null)
                {
                    foreach (var project in request.Projects)
                    {
                        var newProject = new Project
                        {
                            CvId = newCV.CvId,
                            Name = project.Name,
                            Description = project.Description,
                            Technologies = project.Technologies,
                            StartDate = project.StartDate,
                            EndDate = project.EndDate
                        };
                        _context.Projects.Add(newProject);
                    }
                }

                // Create certifications
                if (request.Certifications != null)
                {
                    foreach (var cert in request.Certifications)
                    {
                        var certification = new Certification
                        {
                            CvId = newCV.CvId,
                            Name = cert.Name,
                            Issuer = cert.Issuer,
                            IssueDate = cert.IssueDate,
                            ExpiryDate = cert.ExpiryDate
                        };
                        _context.Certifications.Add(certification);
                    }
                }

                // Create languages
                if (request.Languages != null)
                {
                    foreach (var lang in request.Languages)
                    {
                        var language = new Language
                        {
                            CvId = newCV.CvId,
                            LanguageName = lang.LanguageName,
                            Proficiency = lang.Proficiency
                        };
                        _context.Languages.Add(language);
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Tạo CV thành công",
                    cvId = newCV.CvId,
                    title = newCV.Title
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message, inner = ex.InnerException?.Message });
            }
        }

        /// <summary>
        /// Cập nhật thông tin CV
        /// </summary>
        /// <param name="cvId">ID của CV</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        [HttpPut("{cvId}")]
        public async Task<IActionResult> UpdateCV(int cvId, [FromBody] UpdateCVRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("id")!.Value);
                
                var cv = await _context.Cvs
                    .FirstOrDefaultAsync(c => c.CvId == cvId && c.UserId == userId);

                if (cv == null)
                {
                    return NotFound(new { message = "CV không tồn tại hoặc không thuộc về user này" });
                }

                // Update CV information
                cv.Title = request.Title ?? cv.Title;
                cv.IsPublic = request.IsPublic ?? cv.IsPublic;
                cv.IsDefault = request.IsDefault ?? cv.IsDefault;
                cv.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Cập nhật CV thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message, inner = ex.InnerException?.Message });
            }
        }

        /// <summary>
        /// Xóa CV
        /// </summary>
        /// <param name="cvId">ID của CV</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{cvId}")]
        public async Task<IActionResult> DeleteCV(int cvId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("id")!.Value);
                
                var cv = await _context.Cvs
                    .FirstOrDefaultAsync(c => c.CvId == cvId && c.UserId == userId);

                if (cv == null)
                {
                    return NotFound(new { message = "CV không tồn tại hoặc không thuộc về user này" });
                }

                _context.Cvs.Remove(cv);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Xóa CV thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }
    }
}
