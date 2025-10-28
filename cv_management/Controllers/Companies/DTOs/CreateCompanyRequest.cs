namespace cv_management.Controllers.Companies.DTOs
{
    /// <summary>
    /// DTO để tạo Company profile mới
    /// </summary>
    public class CreateCompanyRequest
    {
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public string? Description { get; set; }
        public string? Website { get; set; }
    }
}

