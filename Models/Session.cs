using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshlyBackendNew.Models
{
    public class Session
    {
        public Session()
        {
            SessionId = Guid.NewGuid();
        }

        // Primary Key
        [Key]
        public Guid SessionId { get; set; }

        // Session Details
        public DateTime CreatedAt { get; set; } // Start time of the session
        public DateTime ExpiredAt { get; set; } // Expiration time of the session
        public string? SessionToken { get; set; }

        // Foreign Keys
        public Guid? UserId { get; set; } // Id of customer/laundry/driver/owner

        // Navigation Properties
        [ForeignKey("UserId")]
        public Customer? Customer { get; set; } 

        [ForeignKey("UserId")]
        public Laundry? Laundry { get; set; } 

        [ForeignKey("UserId")]
        public Driver? Driver { get; set; } 

        [ForeignKey("UserId")]
        public Owner? Owner { get; set; }
    }
}
