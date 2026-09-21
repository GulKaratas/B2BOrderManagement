using System.ComponentModel.DataAnnotations;

namespace B2BOrderManagement.Api.Contracts;

public record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);

public record SupplierDto(
    int Id,
    string Code,
    string CompanyName,
    string TaxNumber,
    string Email,
    string Phone,
    string City,
    string Country,
    string Status,
    DateTime CreatedAt);

public record CreateSupplierRequest(
    [Required, StringLength(20)] string Code,
    [Required, StringLength(200)] string CompanyName,
    [Required, StringLength(20)] string TaxNumber,
    [Required, EmailAddress] string Email,
    [Required, StringLength(30)] string Phone,
    [Required, StringLength(80)] string City,
    [StringLength(2)] string Country = "TR");

public record UpdateSupplierRequest(
    [Required, StringLength(200)] string CompanyName,
    [Required, EmailAddress] string Email,
    [Required, StringLength(30)] string Phone,
    [Required, StringLength(80)] string City,
    [Required] string Status);

public record ProductDto(
    int Id,
    int SupplierId,
    string SupplierName,
    string Sku,
    string Name,
    string? Description,
    string Category,
    decimal UnitPrice,
    string Currency,
    int StockQuantity,
    int MinOrderQuantity,
    string Status);

public record CreateProductRequest(
    [Range(1, int.MaxValue)] int SupplierId,
    [Required, StringLength(40)] string Sku,
    [Required, StringLength(200)] string Name,
    [StringLength(1000)] string? Description,
    [Required, StringLength(80)] string Category,
    [Range(0.01, 9999999)] decimal UnitPrice,
    [StringLength(3)] string Currency,
    [Range(0, int.MaxValue)] int StockQuantity,
    [Range(1, int.MaxValue)] int MinOrderQuantity);

public record UpdateProductRequest(
    [Required, StringLength(200)] string Name,
    [StringLength(1000)] string? Description,
    [Required, StringLength(80)] string Category,
    [Range(0.01, 9999999)] decimal UnitPrice,
    [Range(0, int.MaxValue)] int StockQuantity,
    [Range(1, int.MaxValue)] int MinOrderQuantity,
    [Required] string Status);

public record CustomerDto(int Id, string Code, string CompanyName, string Email, string City, string Status);

public record OrderItemDto(int ProductId, string Sku, string ProductName, int Quantity, decimal UnitPrice, decimal LineTotal);

public record OrderDto(
    int Id,
    string OrderNumber,
    int CustomerId,
    string CustomerName,
    int SupplierId,
    string SupplierName,
    string Status,
    decimal TotalAmount,
    string Currency,
    string Source,
    string? Notes,
    DateTime OrderedAt,
    IReadOnlyList<OrderItemDto> Items);

public record CreateOrderItemRequest(
    [Range(1, int.MaxValue)] int ProductId,
    [Range(1, int.MaxValue)] int Quantity);

public record CreateOrderRequest(
    [Range(1, int.MaxValue)] int CustomerId,
    [Range(1, int.MaxValue)] int SupplierId,
    [MinLength(1)] List<CreateOrderItemRequest> Items,
    [StringLength(500)] string? Notes);

public record IntegrationOrderRequest(
    [Required] string CustomerCode,
    [Required] string ExternalOrderNo,
    [MinLength(1)] List<IntegrationOrderItemRequest> Items,
    [StringLength(500)] string? Notes);

public record IntegrationOrderItemRequest(
    [Required] string Sku,
    [Range(1, int.MaxValue)] int Quantity);

public record IntegrationLogDto(
    int Id,
    string Direction,
    string EventType,
    string? ReferenceNumber,
    string Status,
    string? ErrorMessage,
    DateTime CreatedAt);
