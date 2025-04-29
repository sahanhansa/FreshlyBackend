using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshlyBackendNew.Models
{
    public class Contact
    {
        [Key]
        public Guid ContactId { get; set; }

        [Key]
        [Required]
        public required string ContactNumber { get; set; }

    }
}
