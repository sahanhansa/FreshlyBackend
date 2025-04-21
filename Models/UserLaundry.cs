using System.ComponentModel.DataAnnotations;
namespace FreshlyBackendNew.Models
{
    public class UserLaundry
    {
        [Key]
        public Guid UserLaundryId { get; set; } = Guid.NewGuid();

        // Foreign Key for User
        public Guid UserId { get; set; }
        public User User { get; set; }

        // Foreign Key for Laundry
        public Guid LaundryId { get; set; }
        public Laundry Laundry { get; set; }
    }
}
