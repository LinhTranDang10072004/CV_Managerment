namespace cv_management.Controllers.Jobs.DTOs
{
    /// <summary>
    /// DTO để cập nhật thông tin Job
    /// Tất cả các trường đều optional để có thể update từng phần
    /// </summary>
    public class UpdateJobRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Requirements { get; set; }
        public string? SalaryRange { get; set; }
        public string? Location { get; set; }
        public string? JobType { get; set; }
        public string? Status { get; set; }
    }
}

