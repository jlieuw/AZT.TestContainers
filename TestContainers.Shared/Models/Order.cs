namespace TestContainers.Shared.Models;

using System;

public partial class Order
{
    public int OrderId { get; set; }
    public int? UserId { get; set; }
    public int? ItemId { get; set; }
    public int NumberOfItems { get; set; }
    public DateTime OrderDate { get; set; }

    public virtual Item? Item { get; set; }
    public virtual Account? User { get; set; }
}
