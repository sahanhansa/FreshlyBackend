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

    }
}

