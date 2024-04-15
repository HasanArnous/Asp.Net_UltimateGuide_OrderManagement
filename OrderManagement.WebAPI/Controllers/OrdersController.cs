using Microsoft.AspNetCore.Mvc;
using OrderManagement.Core.DTO;

namespace OrderManagement.WebAPI.Controllers;

public class OrdersController : CustomControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<OrderResponse>>> GetOrders()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{orderId}")]
    public async Task<ActionResult<OrderResponse>> GetOrder(Guid orderId)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<IActionResult> PostOrder(OrderAddRequest orderAddRequest)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{orderId}")]
    public async Task<IActionResult> PutOrder(Guid orderId, OrderUpdateRequest orderAddRequest)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{orderId}")]
    public async Task<IActionResult> DeleteOrder(Guid orderId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{orderId}/Items")]
    public async Task<ActionResult<List<OrderItemResponse>>> GetOrderItems(Guid orderId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{orderId}/Items/{id}")]
    public async Task<ActionResult<OrderItemResponse>> GetOrderItem(Guid orderId, Guid id)
    {
        throw new NotImplementedException();
    }

    [HttpPost("{orderId}/Items")]
    public async Task<IActionResult> PostOrderItem(Guid orderId, OrderItemAddRequest orderItemAddRequest)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{orderId}/Items/{id}")]
    public async Task<ActionResult<OrderItemResponse>> PutOrderItem(Guid orderId, Guid id, OrderItemUpdateRequest orderItemUpdateRequest)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{orderId}/Items/{id}")]
    public async Task<IActionResult> DeleteOrderItem(Guid orderId, Guid id)
    {
        throw new NotImplementedException();
    }

}
