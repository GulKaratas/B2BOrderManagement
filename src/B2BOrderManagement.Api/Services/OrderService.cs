using System.Text.Json;
using B2BOrderManagement.Api.Contracts;
using B2BOrderManagement.Api.Data;
using B2BOrderManagement.Api.Domain;
using B2BOrderManagement.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace B2BOrderManagement.Api.Services;

public class OrderService
{
    private readonly AppDbContext _db;

    public OrderService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<OrderDto>> GetOrdersAsync(string? status, int? supplierId, int page, int pageSize)
    {
        var query = _db.Orders.AsNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.Supplier)
            .Include(x => x.Items).ThenInclude(x => x.Product)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<OrderStatus>(status, true, out var parsed))
        {
            query = query.Where(x => x.Status == parsed);
        }

        if (supplierId.HasValue)
        {
            query = query.Where(x => x.SupplierId == supplierId.Value);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(x => x.OrderedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<OrderDto>(items.Select(Map).ToList(), total, page, pageSize);
    }

    public async Task<OrderDto> GetOrderAsync(int id)
    {
        var order = await LoadOrderAsync(id);
        return Map(order);
    }

    public async Task<OrderDto> CreateDraftAsync(CreateOrderRequest request)
    {
        var order = await BuildOrderAsync(request, OrderStatus.Draft, "Portal", persist: true);
        return Map(order);
    }

    public async Task<OrderDto> SubmitAsync(int id)
    {
        var order = await LoadOrderAsync(id);
        EnsureTransition(order.Status, OrderStatus.Submitted);
        await ValidateStockAsync(order, reserve: false);

        order.Status = OrderStatus.Submitted;
        order.SubmittedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Map(order);
    }

    public async Task<OrderDto> ConfirmAsync(int id)
    {
        var order = await LoadOrderAsync(id);
        EnsureTransition(order.Status, OrderStatus.Confirmed);
        await ValidateStockAsync(order, reserve: true);

        order.Status = OrderStatus.Confirmed;
        order.ConfirmedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Map(order);
    }

    public async Task<OrderDto> RejectAsync(int id)
    {
        var order = await LoadOrderAsync(id);
        EnsureTransition(order.Status, OrderStatus.Rejected);
        order.Status = OrderStatus.Rejected;
        await _db.SaveChangesAsync();
        return Map(order);
    }

    public async Task<OrderDto> ShipAsync(int id)
    {
        var order = await LoadOrderAsync(id);
        EnsureTransition(order.Status, OrderStatus.Shipped);
        order.Status = OrderStatus.Shipped;
        await _db.SaveChangesAsync();
        return Map(order);
    }

    public async Task<OrderDto> CancelAsync(int id)
    {
        var order = await LoadOrderAsync(id);
        EnsureTransition(order.Status, OrderStatus.Cancelled);

        if (order.Status == OrderStatus.Confirmed)
        {
            foreach (var item in order.Items)
            {
                item.Product.StockQuantity += item.Quantity;
            }
        }

        order.Status = OrderStatus.Cancelled;
        order.CancelledAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Map(order);
    }

    public async Task<OrderDto> CreateFromIntegrationAsync(string apiKey, IntegrationOrderRequest request)
    {
        var supplier = await _db.Suppliers.FirstOrDefaultAsync(x => x.ApiKey == apiKey)
            ?? throw BusinessException.Unauthorized("Geçersiz API anahtarı.");

        if (supplier.Status != SupplierStatus.Active)
        {
            throw BusinessException.Conflict("SUPPLIER_INACTIVE", "Tedarikçi entegrasyona kapalı.", new { supplier.Code });
        }

        var payload = JsonSerializer.Serialize(request);
        try
        {
            var customer = await _db.Customers.FirstOrDefaultAsync(x => x.Code == request.CustomerCode)
                ?? throw BusinessException.NotFound("Müşteri", request.CustomerCode);

            var skus = request.Items.Select(i => i.Sku).ToList();
            var products = await _db.Products.Where(x => skus.Contains(x.Sku)).ToListAsync();
            var items = request.Items.Select(item =>
            {
                var product = products.FirstOrDefault(x => x.Sku == item.Sku)
                    ?? throw BusinessException.NotFound("Ürün", item.Sku);
                return new CreateOrderItemRequest(product.Id, item.Quantity);
            }).ToList();

            var createRequest = new CreateOrderRequest(customer.Id, supplier.Id, items, request.Notes);
            var order = await BuildOrderAsync(createRequest, OrderStatus.Submitted, "Integration", persist: true);

            _db.IntegrationLogs.Add(new IntegrationLog
            {
                Direction = "Inbound",
                EventType = "CreateOrder",
                ReferenceNumber = order.OrderNumber,
                Payload = payload,
                Status = IntegrationStatus.Success,
                CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
            return Map(order);
        }
        catch (Exception ex)
        {
            _db.IntegrationLogs.Add(new IntegrationLog
            {
                Direction = "Inbound",
                EventType = "CreateOrder",
                ReferenceNumber = request.ExternalOrderNo,
                Payload = payload,
                Status = IntegrationStatus.Failed,
                ErrorMessage = ex.Message,
                CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
            throw;
        }
    }

    public async Task<PagedResult<IntegrationLogDto>> GetLogsAsync(int page, int pageSize)
    {
        var query = _db.IntegrationLogs.AsNoTracking().OrderByDescending(x => x.CreatedAt);
        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new IntegrationLogDto(x.Id, x.Direction, x.EventType, x.ReferenceNumber, x.Status.ToString(), x.ErrorMessage, x.CreatedAt))
            .ToListAsync();
        return new PagedResult<IntegrationLogDto>(items, total, page, pageSize);
    }

    private async Task<Order> BuildOrderAsync(CreateOrderRequest request, OrderStatus status, string source, bool persist)
    {
        if (request.Items.Count == 0)
        {
            throw BusinessException.Validation("Siparişte en az bir kalem olmalıdır.");
        }

        var customer = await _db.Customers.FirstOrDefaultAsync(x => x.Id == request.CustomerId)
            ?? throw BusinessException.NotFound("Müşteri", request.CustomerId);
        if (customer.Status != CustomerStatus.Active)
        {
            throw BusinessException.Conflict("CUSTOMER_INACTIVE", "Pasif müşteri sipariş oluşturamaz.", new { customer.Code });
        }

        var supplier = await _db.Suppliers.FirstOrDefaultAsync(x => x.Id == request.SupplierId)
            ?? throw BusinessException.NotFound("Tedarikçi", request.SupplierId);
        if (supplier.Status != SupplierStatus.Active)
        {
            throw BusinessException.Conflict("SUPPLIER_INACTIVE", "Pasif tedarikçiden sipariş alınamaz.", new { supplier.Code });
        }

        var productIds = request.Items.Select(x => x.ProductId).Distinct().ToList();
        var products = await _db.Products.Where(x => productIds.Contains(x.Id)).ToListAsync();

        var orderItems = new List<OrderItem>();
        foreach (var line in request.Items)
        {
            var product = products.FirstOrDefault(x => x.Id == line.ProductId)
                ?? throw BusinessException.NotFound("Ürün", line.ProductId);

            if (product.SupplierId != supplier.Id)
            {
                throw BusinessException.Conflict("PRODUCT_SUPPLIER_MISMATCH", "Ürün seçilen tedarikçiye ait değil.", new { product.Sku, supplier.Code });
            }

            if (product.Status != ProductStatus.Active)
            {
                throw BusinessException.Conflict("PRODUCT_INACTIVE", "Pasif veya durdurulmuş ürün sipariş edilemez.", new { product.Sku });
            }

            if (line.Quantity < product.MinOrderQuantity)
            {
                throw BusinessException.Validation("Minimum sipariş adedinin altında.", new { product.Sku, product.MinOrderQuantity, requested = line.Quantity });
            }

            if (line.Quantity > product.StockQuantity)
            {
                throw BusinessException.Conflict("STOCK_INSUFFICIENT", "Ürün stok miktarı yetersiz.", new { product.Sku, requested = line.Quantity, available = product.StockQuantity });
            }

            orderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                Product = product,
                Quantity = line.Quantity,
                UnitPrice = product.UnitPrice,
                LineTotal = product.UnitPrice * line.Quantity
            });
        }

        var order = new Order
        {
            OrderNumber = await NextOrderNumberAsync(),
            CustomerId = customer.Id,
            Customer = customer,
            SupplierId = supplier.Id,
            Supplier = supplier,
            Status = status,
            Currency = "TRY",
            Source = source,
            Notes = request.Notes,
            OrderedAt = DateTime.UtcNow,
            SubmittedAt = status == OrderStatus.Submitted ? DateTime.UtcNow : null,
            Items = orderItems,
            TotalAmount = orderItems.Sum(x => x.LineTotal)
        };

        if (persist)
        {
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
        }

        return order;
    }

    private async Task<Order> LoadOrderAsync(int id)
    {
        return await _db.Orders
            .Include(x => x.Customer)
            .Include(x => x.Supplier)
            .Include(x => x.Items).ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id)
            ?? throw BusinessException.NotFound("Sipariş", id);
    }

    private static async Task ValidateStockAsync(Order order, bool reserve)
    {
        foreach (var item in order.Items)
        {
            if (item.Product.Status != ProductStatus.Active)
            {
                throw BusinessException.Conflict("PRODUCT_INACTIVE", "Pasif ürün onaylanamaz.", new { item.Product.Sku });
            }

            if (item.Quantity > item.Product.StockQuantity)
            {
                throw BusinessException.Conflict("STOCK_INSUFFICIENT", "Ürün stok miktarı yetersiz.", new { item.Product.Sku, requested = item.Quantity, available = item.Product.StockQuantity });
            }

            if (reserve)
            {
                item.Product.StockQuantity -= item.Quantity;
            }
        }

        await Task.CompletedTask;
    }

    private static void EnsureTransition(OrderStatus current, OrderStatus next)
    {
        var allowed = current switch
        {
            OrderStatus.Draft => new[] { OrderStatus.Submitted, OrderStatus.Cancelled },
            OrderStatus.Submitted => new[] { OrderStatus.Confirmed, OrderStatus.Rejected, OrderStatus.Cancelled },
            OrderStatus.Confirmed => new[] { OrderStatus.Shipped, OrderStatus.Cancelled },
            _ => Array.Empty<OrderStatus>()
        };

        if (!allowed.Contains(next))
        {
            throw BusinessException.Conflict("INVALID_STATUS_TRANSITION", "Bu durum geçişine izin verilmiyor.", new { current = current.ToString(), next = next.ToString() });
        }
    }

    private async Task<string> NextOrderNumberAsync()
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        var prefix = $"ORD-{today}-";
        var last = await _db.Orders
            .Where(x => x.OrderNumber.StartsWith(prefix))
            .OrderByDescending(x => x.OrderNumber)
            .Select(x => x.OrderNumber)
            .FirstOrDefaultAsync();

        var seq = 1;
        if (last is not null)
        {
            seq = int.Parse(last[^4..]) + 1;
        }

        return $"{prefix}{seq:0000}";
    }

    private static OrderDto Map(Order order) => new(
        order.Id,
        order.OrderNumber,
        order.CustomerId,
        order.Customer.CompanyName,
        order.SupplierId,
        order.Supplier.CompanyName,
        order.Status.ToString(),
        order.TotalAmount,
        order.Currency,
        order.Source,
        order.Notes,
        order.OrderedAt,
        order.Items.Select(i => new OrderItemDto(i.ProductId, i.Product.Sku, i.Product.Name, i.Quantity, i.UnitPrice, i.LineTotal)).ToList());
}
