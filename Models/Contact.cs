using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshlyBackendNew.Models
{
    public class Contact
    {
        public Contact()
        {
            ContactId = Guid.NewGuid();
        }

        [Key]
        public Guid ContactId { get; set; }

        public string? ContactNumber { get; set; }

        public Guid? UserId { get; set; }

        public string? UserType { get; set; }
    }
}