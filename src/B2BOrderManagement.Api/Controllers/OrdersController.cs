using B2BOrderManagement.Api.Contracts;
using B2BOrderManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace B2BOrderManagement.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orders;

    public OrdersController(OrderService orders)
    {
        _orders = orders;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<OrderDto>>> Get([FromQuery] string? status, [FromQuery] int? supplierId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return Ok(await _orders.GetOrdersAsync(status, supplierId, Math.Max(page, 1), Math.Clamp(pageSize, 1, 100)));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetById(int id) => Ok(await _orders.GetOrderAsync(id));

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderRequest request)
    {
        var created = await _orders.CreateDraftAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPost("{id:int}/submit")]
    public async Task<ActionResult<OrderDto>> Submit(int id) => Ok(await _orders.SubmitAsync(id));

    [HttpPost("{id:int}/confirm")]
    public async Task<ActionResult<OrderDto>> Confirm(int id) => Ok(await _orders.ConfirmAsync(id));

    [HttpPost("{id:int}/reject")]
    public async Task<ActionResult<OrderDto>> Reject(int id) => Ok(await _orders.RejectAsync(id));

    [HttpPost("{id:int}/ship")]
    public async Task<ActionResult<OrderDto>> Ship(int id) => Ok(await _orders.ShipAsync(id));

    [HttpPost("{id:int}/cancel")]
    public async Task<ActionResult<OrderDto>> Cancel(int id) => Ok(await _orders.CancelAsync(id));
}
