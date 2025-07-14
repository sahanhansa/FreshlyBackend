using System;
using System.Collections.Generic;

namespace FreshlyBackendNew.ScaffoldedModels;

public partial class Temporaryorder
{
    public Guid TemporaryOrderId { get; set; }

    public DateTime? PlacedAt { get; set; }

    public DateTime? PickupAt { get; set; }

    public Guid? LaundryId { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? StatusId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Laundry? Laundry { get; set; }

    public virtual Status? Status { get; set; }

    public virtual ICollection<Temporaryorderdetail> Temporaryorderdetails { get; set; } = new List<Temporaryorderdetail>();
}
