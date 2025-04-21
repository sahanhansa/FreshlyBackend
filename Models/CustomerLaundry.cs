using System.ComponentModel.DataAnnotations;
namespace FreshlyBackendNew.Models
{
    public class CustomerLaundry
    {
        [Key]
        public Guid CustomerLaundryId { get; set; } = Guid.NewGuid();

        // Foreign Key for Customer
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }

        // Foreign Key for Laundry
        public Guid LaundryId { get; set; }
        public Laundry Laundry { get; set; }
    }
}
