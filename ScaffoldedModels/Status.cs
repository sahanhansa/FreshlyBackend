using System;
using System.Collections.Generic;

namespace FreshlyBackendNew.ScaffoldedModels;

public partial class Status
{
    public Guid StatusId { get; set; }

    public string? StatusName { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Temporaryorder> Temporaryorders { get; set; } = new List<Temporaryorder>();
}
