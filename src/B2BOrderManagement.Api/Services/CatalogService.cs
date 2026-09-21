using B2BOrderManagement.Api.Contracts;
using B2BOrderManagement.Api.Data;
using B2BOrderManagement.Api.Domain;
using B2BOrderManagement.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace B2BOrderManagement.Api.Services;

public class CatalogService
{
    private readonly AppDbContext _db;

    public CatalogService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<SupplierDto>> GetSuppliersAsync(string? status, int page, int pageSize)
    {
        var query = _db.Suppliers.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<SupplierStatus>(status, true, out var parsed))
        {
            query = query.Where(x => x.Status == parsed);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(x => x.CompanyName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<SupplierDto>(items.Select(Map).ToList(), total, page, pageSize);
    }

    public async Task<SupplierDto> GetSupplierAsync(int id)
    {
        var supplier = await _db.Suppliers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id)
            ?? throw BusinessException.NotFound("Tedarikçi", id);
        return Map(supplier);
    }

    public async Task<SupplierDto> CreateSupplierAsync(CreateSupplierRequest request)
    {
        if (await _db.Suppliers.AnyAsync(x => x.Code == request.Code))
        {
            throw BusinessException.Conflict("DUPLICATE_CODE", "Tedarikçi kodu zaten kayıtlı.", new { request.Code });
        }

        if (await _db.Suppliers.AnyAsync(x => x.TaxNumber == request.TaxNumber))
        {
            throw BusinessException.Conflict("DUPLICATE_TAX_NUMBER", "Vergi numarası zaten kayıtlı.", new { request.TaxNumber });
        }

        var entity = new Supplier
        {
            Code = request.Code.Trim().ToUpperInvariant(),
            CompanyName = request.CompanyName.Trim(),
            TaxNumber = request.TaxNumber.Trim(),
            Email = request.Email.Trim(),
            Phone = request.Phone.Trim(),
            City = request.City.Trim(),
            Country = string.IsNullOrWhiteSpace(request.Country) ? "TR" : request.Country.ToUpperInvariant(),
            Status = SupplierStatus.Active,
            ApiKey = $"sup-{Guid.NewGuid():N}"[..24],
            CreatedAt = DateTime.UtcNow
        };

        _db.Suppliers.Add(entity);
        await _db.SaveChangesAsync();
        return Map(entity);
    }

    public async Task<SupplierDto> UpdateSupplierAsync(int id, UpdateSupplierRequest request)
    {
        var entity = await _db.Suppliers.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw BusinessException.NotFound("Tedarikçi", id);

        if (!Enum.TryParse<SupplierStatus>(request.Status, true, out var status))
        {
            throw BusinessException.Validation("Geçersiz tedarikçi durumu.", new { request.Status });
        }

        entity.CompanyName = request.CompanyName.Trim();
        entity.Email = request.Email.Trim();
        entity.Phone = request.Phone.Trim();
        entity.City = request.City.Trim();
        entity.Status = status;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Map(entity);
    }

    public async Task<PagedResult<ProductDto>> GetProductsAsync(int? supplierId, string? status, int page, int pageSize)
    {
        var query = _db.Products.AsNoTracking().Include(x => x.Supplier).AsQueryable();
        if (supplierId.HasValue)
        {
            query = query.Where(x => x.SupplierId == supplierId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<ProductStatus>(status, true, out var parsed))
        {
            query = query.Where(x => x.Status == parsed);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(x => x.Sku)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<ProductDto>(items.Select(Map).ToList(), total, page, pageSize);
    }

    public async Task<ProductDto> GetProductAsync(int id)
    {
        var product = await _db.Products.AsNoTracking().Include(x => x.Supplier).FirstOrDefaultAsync(x => x.Id == id)
            ?? throw BusinessException.NotFound("Ürün", id);
        return Map(product);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductRequest request)
    {
        var supplier = await _db.Suppliers.FirstOrDefaultAsync(x => x.Id == request.SupplierId)
            ?? throw BusinessException.NotFound("Tedarikçi", request.SupplierId);

        if (supplier.Status != SupplierStatus.Active)
        {
            throw BusinessException.Conflict("SUPPLIER_INACTIVE", "Pasif tedarikçiye ürün eklenemez.", new { supplier.Code });
        }

        if (await _db.Products.AnyAsync(x => x.Sku == request.Sku))
        {
            throw BusinessException.Conflict("DUPLICATE_SKU", "SKU zaten kayıtlı.", new { request.Sku });
        }

        var entity = new Product
        {
            SupplierId = supplier.Id,
            Sku = request.Sku.Trim().ToUpperInvariant(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Category = request.Category.Trim(),
            UnitPrice = request.UnitPrice,
            Currency = string.IsNullOrWhiteSpace(request.Currency) ? "TRY" : request.Currency.ToUpperInvariant(),
            StockQuantity = request.StockQuantity,
            MinOrderQuantity = request.MinOrderQuantity,
            Status = ProductStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _db.Products.Add(entity);
        await _db.SaveChangesAsync();
        entity.Supplier = supplier;
        return Map(entity);
    }

    public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductRequest request)
    {
        var entity = await _db.Products.Include(x => x.Supplier).FirstOrDefaultAsync(x => x.Id == id)
            ?? throw BusinessException.NotFound("Ürün", id);

        if (!Enum.TryParse<ProductStatus>(request.Status, true, out var status))
        {
            throw BusinessException.Validation("Geçersiz ürün durumu.", new { request.Status });
        }

        entity.Name = request.Name.Trim();
        entity.Description = request.Description?.Trim();
        entity.Category = request.Category.Trim();
        entity.UnitPrice = request.UnitPrice;
        entity.StockQuantity = request.StockQuantity;
        entity.MinOrderQuantity = request.MinOrderQuantity;
        entity.Status = status;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Map(entity);
    }

    public async Task<IReadOnlyList<CustomerDto>> GetCustomersAsync()
    {
        return await _db.Customers.AsNoTracking()
            .OrderBy(x => x.CompanyName)
            .Select(x => new CustomerDto(x.Id, x.Code, x.CompanyName, x.Email, x.City, x.Status.ToString()))
            .ToListAsync();
    }

    private static SupplierDto Map(Supplier x) =>
        new(x.Id, x.Code, x.CompanyName, x.TaxNumber, x.Email, x.Phone, x.City, x.Country, x.Status.ToString(), x.CreatedAt);

    private static ProductDto Map(Product x) =>
        new(x.Id, x.SupplierId, x.Supplier.CompanyName, x.Sku, x.Name, x.Description, x.Category, x.UnitPrice, x.Currency, x.StockQuantity, x.MinOrderQuantity, x.Status.ToString());
}
