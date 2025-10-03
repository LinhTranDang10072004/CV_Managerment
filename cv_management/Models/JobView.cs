using System;
using System.Collections.Generic;

namespace cv_management.Models;

public partial class JobView
{
    public int ViewId { get; set; }

    public int JobId { get; set; }

    public int? UserId { get; set; }

    public DateTime? ViewedAt { get; set; }

    public virtual Job Job { get; set; } = null!;

    public virtual User? User { get; set; }
}
