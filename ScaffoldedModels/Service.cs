using System;
using System.Collections.Generic;

namespace FreshlyBackendNew.ScaffoldedModels;

public partial class Service
{
    public Guid ServiceId { get; set; }

    public string? ServiceName { get; set; }

    public virtual ICollection<Laundryitemservice> Laundryitemservices { get; set; } = new List<Laundryitemservice>();

    public virtual ICollection<Orderdetail> Orderdetails { get; set; } = new List<Orderdetail>();

    public virtual ICollection<Temporaryorderdetail> Temporaryorderdetails { get; set; } = new List<Temporaryorderdetail>();
}
