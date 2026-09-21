using B2BOrderManagement.Api.Contracts;
using B2BOrderManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace B2BOrderManagement.Api.Controllers;

[ApiController]
[Route("api/suppliers")]
public class SuppliersController : ControllerBase
{
    private readonly CatalogService _catalog;

    public SuppliersController(CatalogService catalog)
    {
        _catalog = catalog;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<SupplierDto>>> Get([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return Ok(await _catalog.GetSuppliersAsync(status, Math.Max(page, 1), Math.Clamp(pageSize, 1, 100)));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SupplierDto>> GetById(int id) => Ok(await _catalog.GetSupplierAsync(id));

    [HttpPost]
    public async Task<ActionResult<SupplierDto>> Create([FromBody] CreateSupplierRequest request)
    {
        var created = await _catalog.CreateSupplierAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SupplierDto>> Update(int id, [FromBody] UpdateSupplierRequest request) =>
        Ok(await _catalog.UpdateSupplierAsync(id, request));
}
