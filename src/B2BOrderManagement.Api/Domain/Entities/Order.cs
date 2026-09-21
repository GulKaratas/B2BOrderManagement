namespace B2BOrderManagement.Api.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public int SupplierId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "TRY";
    public string Source { get; set; } = "Portal";
    public string? Notes { get; set; }
    public DateTime OrderedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public Customer Customer { get; set; } = null!;
    public Supplier Supplier { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
