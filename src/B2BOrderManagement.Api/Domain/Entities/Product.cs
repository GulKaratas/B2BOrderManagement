namespace B2BOrderManagement.Api.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public string Currency { get; set; } = "TRY";
    public int StockQuantity { get; set; }
    public int MinOrderQuantity { get; set; } = 1;
    public ProductStatus Status { get; set; } = ProductStatus.Active;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Supplier Supplier { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
