using System;
using System.Collections.Generic;

namespace FreshlyBackendNew.ScaffoldedModels;

public partial class Laundryitemservice
{
    public Guid LaundryId { get; set; }

    public Guid ItemId { get; set; }

    public Guid ServiceId { get; set; }

    public decimal? Price { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual Laundry Laundry { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
