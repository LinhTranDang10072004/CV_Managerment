namespace cv_management.Controllers.CV.DTOs
{
    public class CreateCVRequest
    {
        public string Title { get; set; } = null!;
        public bool? IsPublic { get; set; }
        public bool? IsDefault { get; set; }
        public PersonalInfoDto? PersonalInfo { get; set; }
        public List<WorkExperienceDto>? WorkExperiences { get; set; }
        public List<EducationDto>? Educations { get; set; }
        public List<SkillDto>? Skills { get; set; }
        public List<ProjectDto>? Projects { get; set; }
        public List<CertificationDto>? Certifications { get; set; }
        public List<LanguageDto>? Languages { get; set; }
    }

    public class PersonalInfoDto
    {
        public string FullName { get; set; } = null!;
        public DateOnly? DateOfBirth { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Summary { get; set; }
    }

    public class WorkExperienceDto
    {
        public string CompanyName { get; set; } = null!;
        public string Position { get; set; } = null!;
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Description { get; set; }
    }

    public class EducationDto
    {
        public string SchoolName { get; set; } = null!;
        public string? Degree { get; set; }
        public string? Major { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Description { get; set; }
    }

    public class SkillDto
    {
        public string SkillName { get; set; } = null!;
        public string? Proficiency { get; set; }
    }

    public class ProjectDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Technologies { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }

    public class CertificationDto
    {
        public string Name { get; set; } = null!;
        public string? Issuer { get; set; }
        public DateOnly? IssueDate { get; set; }
        public DateOnly? ExpiryDate { get; set; }
    }

    public class LanguageDto
    {
        public string LanguageName { get; set; } = null!;
        public string? Proficiency { get; set; }
    }
}
