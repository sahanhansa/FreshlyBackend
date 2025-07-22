using System;
using System.ComponentModel.DataAnnotations;

namespace FreshlyBackendNew.Models
{
    public class GarmentType
    {
        [Key]
        public Guid GarmentTypeId { get; set; }

        public string? GarmentTypeName { get; set; }
    }
}