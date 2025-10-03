using System;
using System.Collections.Generic;

namespace cv_management.Models;

public partial class Education
{
    public int EducationId { get; set; }

    public int CvId { get; set; }

    public string SchoolName { get; set; } = null!;

    public string? Degree { get; set; }

    public string? Major { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Description { get; set; }

    public virtual Cv Cv { get; set; } = null!;
}
