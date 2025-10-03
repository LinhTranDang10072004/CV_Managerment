using System;
using System.Collections.Generic;

namespace cv_management.Models;

public partial class Job
{
    public int JobId { get; set; }

    public int CompanyId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? Requirements { get; set; }

    public string? SalaryRange { get; set; }

    public string? Location { get; set; }

    public string? JobType { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<JobView> JobViews { get; set; } = new List<JobView>();

    public virtual ICollection<SavedJob> SavedJobs { get; set; } = new List<SavedJob>();
}
