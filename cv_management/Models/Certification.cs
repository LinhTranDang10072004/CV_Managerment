using System;
using System.Collections.Generic;

namespace cv_management.Models;

public partial class Certification
{
    public int CertificationId { get; set; }

    public int CvId { get; set; }

    public string Name { get; set; } = null!;

    public string? Issuer { get; set; }

    public DateOnly? IssueDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public virtual Cv Cv { get; set; } = null!;
}
