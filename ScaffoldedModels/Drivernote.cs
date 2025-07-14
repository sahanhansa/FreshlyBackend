using System;
using System.Collections.Generic;

namespace FreshlyBackendNew.ScaffoldedModels;

public partial class Drivernote
{
    public Guid NoteId { get; set; }

    public string? Note { get; set; }

    public Guid? DriverId { get; set; }

    public Guid? OrderId { get; set; }

    public virtual Driver? Driver { get; set; }

    public virtual Order? Order { get; set; }
}
