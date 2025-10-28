namespace cv_management.Controllers.Companies.DTOs
{
    /// <summary>
    /// DTO trả về thông tin Company
    /// </summary>
    public class CompanyResponse
    {
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Description { get; set; }
        public string? Website { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

