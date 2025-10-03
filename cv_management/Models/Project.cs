using System;
using System.Collections.Generic;

namespace cv_management.Models;

public partial class Project
{
    public int ProjectId { get; set; }

    public int CvId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Technologies { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public virtual Cv Cv { get; set; } = null!;
}
