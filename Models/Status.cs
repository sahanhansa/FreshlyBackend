using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class Status
    {
        public Status()
        {
            StatusID = Guid.NewGuid(); 
        }

        // Primary Key
        [Key]
        public Guid StatusID { get; set; }

        // Status Details
        public string? StatusName { get; set; }

        //Followings are the finalized order statuses:
        //-Order placed
        //-Order pickup scheduled
        //-Picked up
        //-Processing in laundry
        //-Finished processing
        //-Out for delivery
        //-Completed
    }
}
