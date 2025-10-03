using System;
using System.Collections.Generic;

namespace cv_management.Models;

public partial class Skill
{
    public int SkillId { get; set; }

    public int CvId { get; set; }

    public string SkillName { get; set; } = null!;

    public string? Proficiency { get; set; }

    public virtual Cv Cv { get; set; } = null!;
}
