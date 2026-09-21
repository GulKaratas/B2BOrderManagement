using B2BOrderManagement.Api.Contracts;
using B2BOrderManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace B2BOrderManagement.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly CatalogService _catalog;

    public CustomersController(CatalogService catalog)
    {
        _catalog = catalog;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerDto>>> Get() => Ok(await _catalog.GetCustomersAsync());
}
