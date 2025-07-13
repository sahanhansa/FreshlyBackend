using System;
using System.Collections.Generic;

namespace FreshlyBackendNew.ScaffoldedModels;

public partial class Feedback
{
    public Guid FeedbackId { get; set; }

    public string? Description { get; set; }

    public int? Rating { get; set; }

    public Guid? OrderId { get; set; }

    public virtual Order? Order { get; set; }
}
