using System;
using System.Collections.Generic;

namespace cv_management.Models;

public partial class Application
{
    public int ApplicationId { get; set; }

    public int CvId { get; set; }

    public int JobId { get; set; }

    public DateTime? AppliedAt { get; set; }

    public string? Status { get; set; }

    public string? CoverLetter { get; set; }

    public virtual Cv Cv { get; set; } = null!;

    public virtual Job Job { get; set; } = null!;
}
