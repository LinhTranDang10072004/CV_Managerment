namespace cv_management.Controllers.User.DTOs
{
    public class UpdateProfileRequest
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
