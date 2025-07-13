using System;
using System.Collections.Generic;

namespace FreshlyBackendNew.ScaffoldedModels;

public partial class Orderdetail
{
    public Guid OrderId { get; set; }

    public Guid ItemId { get; set; }

    public Guid ServiceId { get; set; }

    public int? Quantity { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
