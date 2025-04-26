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
    }
}
