using System;
using System.Collections.Generic;

namespace FreshlyBackendNew.ScaffoldedModels;

public partial class Address
{
    public Guid AddressId { get; set; }

    public string? HouseNo { get; set; }

    public string? Street { get; set; }

    public string? City { get; set; }

    public string? PostalCode { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<Driver> Drivers { get; set; } = new List<Driver>();

    public virtual ICollection<Laundry> Laundries { get; set; } = new List<Laundry>();

    public virtual ICollection<Owner> Owners { get; set; } = new List<Owner>();
}
