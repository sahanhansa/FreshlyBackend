using System.ComponentModel.DataAnnotations;
namespace FreshlyBackendNew.Models
{
    public class UserCustomer
    {
        [Key]
        public Guid UserCustomerId { get; set; } = Guid.NewGuid();

        // Foreign Key for User
        public Guid UserId { get; set; }
        public User User { get; set; }

        // Foreign Key for Customer
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
