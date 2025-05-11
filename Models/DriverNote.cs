using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshlyBackendNew.Models
{
    public class DriverNote
    {
        public DriverNote()
        {
            NoteId = Guid.NewGuid();
        }

        // Primary Key
        [Key]
        public Guid NoteId { get; set; }

        // Basic Information
        public string? Note { get; set; } 

        //Foerign keys
        public Guid? DriverId { get; set; } 
        public Guid? OrderId { get; set; }

        // Navigation Properties
        [ForeignKey("DriverId")]
        public Driver? Driver { get; set; } 

        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

    }
}
