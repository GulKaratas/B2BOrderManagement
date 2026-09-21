using B2BOrderManagement.Api.Contracts;
using B2BOrderManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace B2BOrderManagement.Api.Controllers;

[ApiController]
[Route("api/integrations")]
public class IntegrationsController : ControllerBase
{
    private readonly OrderService _orders;

    public IntegrationsController(OrderService orders)
    {
        _orders = orders;
    }

    [HttpPost("orders")]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromHeader(Name = "X-Api-Key")] string? apiKey, [FromBody] IntegrationOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return Unauthorized(new { code = "UNAUTHORIZED", message = "X-Api-Key başlığı zorunludur." });
        }

        var created = await _orders.CreateFromIntegrationAsync(apiKey, request);
        return CreatedAtAction("GetById", "Orders", new { id = created.Id }, created);
    }

    [HttpGet("logs")]
    public async Task<ActionResult<PagedResult<IntegrationLogDto>>> GetLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return Ok(await _orders.GetLogsAsync(Math.Max(page, 1), Math.Clamp(pageSize, 1, 100)));
    }
}
