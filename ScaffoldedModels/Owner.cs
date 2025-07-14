using System;
using System.Collections.Generic;

namespace FreshlyBackendNew.ScaffoldedModels;

public partial class Owner
{
    public Guid OwnerId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public Guid? AddressId { get; set; }

    public virtual Address? Address { get; set; }

    public virtual ICollection<Laundry> Laundries { get; set; } = new List<Laundry>();

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
