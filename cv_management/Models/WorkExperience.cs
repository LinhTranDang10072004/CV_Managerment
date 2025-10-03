using System;
using System.Collections.Generic;

namespace cv_management.Models;

public partial class WorkExperience
{
    public int ExperienceId { get; set; }

    public int CvId { get; set; }

    public string CompanyName { get; set; } = null!;

    public string Position { get; set; } = null!;

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Description { get; set; }

    public virtual Cv Cv { get; set; } = null!;
}
