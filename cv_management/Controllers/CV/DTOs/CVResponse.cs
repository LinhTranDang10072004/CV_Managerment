namespace cv_management.Controllers.CV.DTOs
{
    public class CVListResponse
    {
        public int CvId { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool? IsPublic { get; set; }
        public bool? IsDefault { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CVDetailsResponse
    {
        public CVInfo Cv { get; set; } = new CVInfo();
        public PersonalInfoResponse? PersonalInfo { get; set; }
        public List<WorkExperienceResponse> WorkExperiences { get; set; } = new List<WorkExperienceResponse>();
        public List<EducationResponse> Educations { get; set; } = new List<EducationResponse>();
        public List<SkillResponse> Skills { get; set; } = new List<SkillResponse>();
        public List<ProjectResponse> Projects { get; set; } = new List<ProjectResponse>();
        public List<CertificationResponse> Certifications { get; set; } = new List<CertificationResponse>();
        public List<LanguageResponse> Languages { get; set; } = new List<LanguageResponse>();
    }

    public class CVInfo
    {
        public int CvId { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool? IsPublic { get; set; }
        public bool? IsDefault { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class PersonalInfoResponse
    {
        public int PersonalInfoId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateOnly? DateOfBirth { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Summary { get; set; }
    }

    public class WorkExperienceResponse
    {
        public int ExperienceId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Description { get; set; }
    }

    public class EducationResponse
    {
        public int EducationId { get; set; }
        public string SchoolName { get; set; } = string.Empty;
        public string? Degree { get; set; }
        public string? Major { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Description { get; set; }
    }

    public class SkillResponse
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string? Proficiency { get; set; }
    }

    public class ProjectResponse
    {
        public int ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Technologies { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }

    public class CertificationResponse
    {
        public int CertificationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Issuer { get; set; }
        public DateOnly? IssueDate { get; set; }
        public DateOnly? ExpiryDate { get; set; }
    }

    public class LanguageResponse
    {
        public int LanguageId { get; set; }
        public string LanguageName { get; set; } = string.Empty;
        public string? Proficiency { get; set; }
    }
}
