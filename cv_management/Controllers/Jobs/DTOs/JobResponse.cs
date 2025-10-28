namespace cv_management.Controllers.Jobs.DTOs
{
    /// <summary>
    /// DTO trả về thông tin Job cho client
    /// Bao gồm cả thông tin Company để client hiển thị
    /// </summary>
    public class JobResponse
    {
        public int JobId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Requirements { get; set; }
        public string? SalaryRange { get; set; }
        public string? Location { get; set; }
        public string? JobType { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO trả về danh sách Jobs với phân trang
    /// </summary>
    public class JobListResponse
    {
        public List<JobResponse> Jobs { get; set; } = new List<JobResponse>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}

