using System;
using System.Collections.Generic;

namespace FreshlyBackendNew.ScaffoldedModels;

public partial class Laundry
{
    public Guid LaundryId { get; set; }

    public string? LaundryName { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public Guid? AddressId { get; set; }

    public Guid? OwnerId { get; set; }

    public virtual Address? Address { get; set; }

    public virtual ICollection<Laundryitemservice> Laundryitemservices { get; set; } = new List<Laundryitemservice>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Owner? Owner { get; set; }

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    public virtual ICollection<Temporaryorder> Temporaryorders { get; set; } = new List<Temporaryorder>();
}
