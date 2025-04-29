using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Status
    {
        public Status()
        {
            StatusID = Guid.NewGuid(); 
            Orders = new List<Order>();
        }

        // Primary Key
        [Key]
        public Guid StatusID { get; set; }

        // Status Details
        public string? StatusName { get; set; }

        // Navigation Properties
        public ICollection<Order>? Orders { get; set; }
    }
}
