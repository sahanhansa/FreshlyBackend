namespace FreshlyBackendNew.DTOs.Driver_DTOs
{
    public class UpdatePasswordDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public Guid DriverId { get; set; }
    }
}
