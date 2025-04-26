using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Service
    {
        public Service()
        {
            ServiceId = Guid.NewGuid();
            
        }

        [Key]
        public Guid ServiceId { get; set; }

        [Required]
        public required string Name { get; set; }

        // Navigation property for OrderDetails
        public ICollection<OrderDetails> OrderDetails { get; set; }

        // Navigation property for LaundryItemService
        public ICollection<LaundryItemService> LaundryItemServices { get; set; }


    }
}

