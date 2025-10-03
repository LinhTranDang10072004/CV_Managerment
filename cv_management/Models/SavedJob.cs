using System;
using System.Collections.Generic;

namespace cv_management.Models;

public partial class SavedJob
{
    public int SavedId { get; set; }

    public int UserId { get; set; }

    public int JobId { get; set; }

    public DateTime? SavedAt { get; set; }

    public virtual Job Job { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
