namespace cv_management.Controllers.Jobs.DTOs
{
    /// <summary>
    /// DTO để tạo Job mới
    /// Company sẽ gửi thông tin này khi muốn đăng tuyển
    /// </summary>
    public class CreateJobRequest
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? Requirements { get; set; }
        public string? SalaryRange { get; set; }
        public string? Location { get; set; }
        public string? JobType { get; set; } // Full-time, Part-time, Contract, Internship
        public string Status { get; set; } = "Active"; // Active, Closed, Draft
    }
}

