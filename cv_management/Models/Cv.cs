using System;
using System.Collections.Generic;

namespace cv_management.Models;

public partial class Cv
{
    public int CvId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public bool? IsPublic { get; set; }

    public bool? IsDefault { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual ICollection<Certification> Certifications { get; set; } = new List<Certification>();

    public virtual ICollection<Education> Educations { get; set; } = new List<Education>();

    public virtual ICollection<Language> Languages { get; set; } = new List<Language>();

    public virtual ICollection<PersonalInfo> PersonalInfos { get; set; } = new List<PersonalInfo>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<WorkExperience> WorkExperiences { get; set; } = new List<WorkExperience>();
}
