using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Status
    {
        public Status()
        {
            StatusID = Guid.NewGuid();
        }

        [Key]
        public Guid StatusID { get; set; }

        [Required]
        public required string StatusName { get; set; }

        // Navigation property for one-to-many relationship
        public ICollection<Order> Orders { get; set; }

    }
}
