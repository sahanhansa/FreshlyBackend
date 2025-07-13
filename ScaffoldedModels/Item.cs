using System;
using System.Collections.Generic;

namespace FreshlyBackendNew.ScaffoldedModels;

public partial class Item
{
    public Guid ItemId { get; set; }

    public string? Name { get; set; }

    public Guid CategoryId { get; set; }

    public virtual Itemcategory Category { get; set; } = null!;

    public virtual ICollection<Laundryitemservice> Laundryitemservices { get; set; } = new List<Laundryitemservice>();

    public virtual ICollection<Orderdetail> Orderdetails { get; set; } = new List<Orderdetail>();

    public virtual ICollection<Temporaryorderdetail> Temporaryorderdetails { get; set; } = new List<Temporaryorderdetail>();
}
