using System;
using System.Collections.Generic;

namespace FreshlyBackendNew.ScaffoldedModels;

public partial class Contact
{
    public Guid ContactId { get; set; }

    public string ContactNumber { get; set; } = null!;

    public Guid? UserId { get; set; }

    public string? UserType { get; set; }
}
