using System;
using System.Collections.Generic;

namespace FreshlyBackendNew.ScaffoldedModels;

public partial class Driver
{
    public Guid DriverId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public string? LicensNo { get; set; }

    public Guid? AddressId { get; set; }

    public virtual Address? Address { get; set; }

    public virtual ICollection<Drivernote> Drivernotes { get; set; } = new List<Drivernote>();

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
