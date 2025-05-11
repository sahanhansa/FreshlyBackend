using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Service
    {
        public Service()
        {
            ServiceId = Guid.NewGuid();
        }

        // Primary Key
        [Key]
        public Guid ServiceId { get; set; }

        // Service Details
        public string? ServiceName { get; set; }

        //Followings are the finalized services of laundries:
        //-Regular Wash
        //-Dry Clean
        //-Hand wash
        //-Press Only
        
    }
}

