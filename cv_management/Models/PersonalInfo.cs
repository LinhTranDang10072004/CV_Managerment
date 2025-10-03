using System;
using System.Collections.Generic;

namespace cv_management.Models;

public partial class PersonalInfo
{
    public int PersonalInfoId { get; set; }

    public int CvId { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? Summary { get; set; }

    public virtual Cv Cv { get; set; } = null!;
}
