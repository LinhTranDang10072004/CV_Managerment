using System;
using System.Collections.Generic;

namespace cv_management.Models;

public partial class Language
{
    public int LanguageId { get; set; }

    public int CvId { get; set; }

    public string LanguageName { get; set; } = null!;

    public string? Proficiency { get; set; }

    public virtual Cv Cv { get; set; } = null!;
}
