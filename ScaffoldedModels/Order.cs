using System;
using System.Collections.Generic;

namespace FreshlyBackendNew.ScaffoldedModels;

public partial class Order
{
    public Guid OrderId { get; set; }

    public DateTime? PlacedAt { get; set; }

    public DateTime? PickupAt { get; set; }

    public Guid? LaundryId { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? StatusId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<Drivernote> Drivernotes { get; set; } = new List<Drivernote>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual Laundry? Laundry { get; set; }

    public virtual ICollection<Orderdetail> Orderdetails { get; set; } = new List<Orderdetail>();

    public virtual Status? Status { get; set; }
}
