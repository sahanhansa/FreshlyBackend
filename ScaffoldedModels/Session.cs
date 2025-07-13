using System;
using System.Collections.Generic;

namespace FreshlyBackendNew.ScaffoldedModels;

public partial class Session
{
    public Guid SessionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiredAt { get; set; }

    public string? SessionToken { get; set; }

    public Guid? UserId { get; set; }

    public virtual Customer? User { get; set; }

    public virtual Laundry? User1 { get; set; }

    public virtual Owner? User2 { get; set; }

    public virtual Driver? UserNavigation { get; set; }
}
