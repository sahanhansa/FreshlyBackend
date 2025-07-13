using System;
using System.Collections.Generic;

namespace FreshlyBackendNew.ScaffoldedModels;

public partial class Customer
{
    public Guid CustomerId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public Guid? AddressId { get; set; }

    public virtual Address? Address { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    public virtual ICollection<Temporaryorder> Temporaryorders { get; set; } = new List<Temporaryorder>();
}
