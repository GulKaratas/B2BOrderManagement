using B2BOrderManagement.Api.Contracts;
using B2BOrderManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace B2BOrderManagement.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly CatalogService _catalog;

    public ProductsController(CatalogService catalog)
    {
        _catalog = catalog;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductDto>>> Get([FromQuery] int? supplierId, [FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return Ok(await _catalog.GetProductsAsync(supplierId, status, Math.Max(page, 1), Math.Clamp(pageSize, 1, 100)));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(int id) => Ok(await _catalog.GetProductAsync(id));

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductRequest request)
    {
        var created = await _catalog.CreateProductAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductRequest request) =>
        Ok(await _catalog.UpdateProductAsync(id, request));
}
