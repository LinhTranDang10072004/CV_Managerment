namespace cv_management.Controllers.Companies.DTOs
{
    /// <summary>
    /// DTO để cập nhật Company profile
    /// </summary>
    public class UpdateCompanyRequest
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public string? Website { get; set; }
    }
}

