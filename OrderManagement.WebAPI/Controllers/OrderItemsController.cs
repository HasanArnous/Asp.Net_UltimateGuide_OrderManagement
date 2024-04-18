using OrderManagement.Core.ServiceContracts.OrderItems;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Core.DTO;

namespace OrderManagement.WebAPI.Controllers;

[ApiController]
[Route("/api/Orders/{orderId}/Items")]
public class OrderItemsController : ControllerBase
{
    private readonly ILogger<OrderItemsController> _logger;
    private readonly IOrderItemsAdderService _orderItemsAdderService;
    private readonly IOrderItemsDeleterService _orderItemsDeleterService;
    private readonly IOrderItemsGetterService _orderItemsGetterService;
    private readonly IOrderItemsUpdaterService _orderItemsUpdaterService;

    public OrderItemsController(ILogger<OrderItemsController> logger, 
        IOrderItemsAdderService orderItemsAdderService, 
        IOrderItemsDeleterService orderItemsDeleterService, 
        IOrderItemsGetterService orderItemsGetterService, 
        IOrderItemsUpdaterService orderItemsUpdaterService)
    {
        _logger = logger;
        _orderItemsAdderService = orderItemsAdderService;
        _orderItemsDeleterService = orderItemsDeleterService;
        _orderItemsGetterService = orderItemsGetterService;
        _orderItemsUpdaterService = orderItemsUpdaterService;
    }

    [HttpGet]
    [ProducesResponseType<ActionResult<List<OrderItemResponse>>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OrderItemResponse>>> GetOrderItems(Guid orderId)
    {
        return Ok(await _orderItemsGetterService.GetAsyncByOrderId(orderId));
    }

    [HttpGet("{id}")]
    [ProducesResponseType<ActionResult<OrderItemResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderItemResponse>> GetOrderItem(Guid orderId, Guid id)
    {
        var orderItem = await _orderItemsGetterService.GetAsyncByOrderItemId(id);
        if(orderItem == null)
        {
            _logger.LogWarning($"GetOrderItem - Failed (Not Found) - Order ID: {orderId}, Order Item ID: {id}");
            return NotFound();
        }
        return Ok(orderItem);
    }

    [HttpPost]
    [ProducesResponseType<ActionResult<OrderItemResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderItemResponse>> PostOrderItem(Guid orderId, OrderItemAddRequest orderItemAddRequest)
    {
        if(orderId != orderItemAddRequest.OrderId)
        {
            _logger.LogWarning($"PostOrderItem - Failed - No match between the supplied route for the OrderId({orderId}), and the OrderItem in the request body({orderItemAddRequest.OrderId})");
            return BadRequest();
        }
        var createdItem = await _orderItemsAdderService.AddAsync(orderItemAddRequest);
        return CreatedAtAction(nameof(GetOrderItem), new { orderId = orderId, id = createdItem.OrderItemId }, createdItem);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<OrderItemResponse>> PutOrderItem(Guid orderId, Guid id, OrderItemUpdateRequest orderItemUpdateRequest)
    {
        if(id != orderItemUpdateRequest.OrderItemId || orderId != orderItemUpdateRequest.OrderId)
        {
            _logger.LogWarning($"PutOrderItem - Failed - No Match between the supplied IDs in the Request Body and the Route values" +
                $"{Environment.NewLine}" +
                $"OrderId Route: {orderId}, OrderId Body: {orderItemUpdateRequest.OrderId}{Environment.NewLine}"+
                $"OrderItemId Route: {id}, OrderIdItem Body: {orderItemUpdateRequest.OrderItemId}");
            return BadRequest();
        }
        var updatedItem = await _orderItemsUpdaterService.UpdateAsync(orderItemUpdateRequest);
        if(updatedItem == null)
        {
            _logger.LogWarning($"PutOrderItem - Failed - Order Item Not Found, Order ID: {orderId}, Order Item Id: {id}");
            return BadRequest();
        }
        _logger.LogInformation($"PutOrderItem - Success - OrderId: {orderId}, OrderItemId: {id}");
        return Ok(updatedItem);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<bool>> DeleteOrderItem(Guid orderId, Guid id)
    {
        var result = await _orderItemsDeleterService.DeleteAsync(id);
        if (result)
        {
            _logger.LogInformation($"DeleteOrderItem - Success - OrderId: {orderId}, OrderItemId: {id}");
            return NoContent();
        }
        _logger.LogInformation($"DeleteOrderItem - Failed - OrderId: {orderId}, OrderItemId: {id}");
        return NotFound();
    }
}
