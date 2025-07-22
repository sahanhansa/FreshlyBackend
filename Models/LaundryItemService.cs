using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshlyBackendNew.Models
{
    public class LaundryItemService
    {
        // Composite Primary Key
        public Guid? LaundryId { get; set; }
        public Guid? ItemId { get; set; }
        public Guid? ServiceId { get; set; }
        public Guid? GarmentTypeId { get; set; }
        //public Guid? MaterialId { get; set; }

        // Service Details
        public decimal? Price { get; set; }

        // Navigation Properties
        [ForeignKey("LaundryId")]
        public Laundry? Laundry { get; set; } 

        [ForeignKey("ItemId")]
        public Item? Item { get; set; } 

        [ForeignKey("ServiceId")]
        public Service? Service { get; set; }

        [ForeignKey("GarmentTypeId")]
        public GarmentType? GarmentType { get; set; }

        //[ForeignKey("MaterialId")]
        //public Material? Material { get; set; }
    }
}
