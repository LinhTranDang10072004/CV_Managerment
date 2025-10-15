namespace cv_management.Controllers.CV.DTOs
{
    public class UpdateCVRequest
    {
        public string? Title { get; set; }
        public bool? IsPublic { get; set; }
        public bool? IsDefault { get; set; }
    }
}
