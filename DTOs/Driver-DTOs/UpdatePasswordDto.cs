namespace FreshlyBackendNew.DTOs.Driver_DTOs
{
    public class UpdatePasswordDto
    {
        public Guid DriverId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
