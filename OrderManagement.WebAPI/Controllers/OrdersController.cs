using OrderManagement.Core.ServiceContracts.Orders;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Core.DTO;

namespace OrderManagement.WebAPI.Controllers;

public class OrdersController : CustomControllerBase
{
    private readonly ILogger<OrdersController> _logger;
    private readonly IOrdersAdderService _ordersAdderService;
    private readonly IOrdersDeleterService _ordersDeleterService;
    private readonly IOrdersGetterService _ordersGetterService;
    private readonly IOrdersUpdaterService _ordersUpdaterService;

    public OrdersController(ILogger<OrdersController> logger, 
        IOrdersAdderService ordersAdderService, 
        IOrdersDeleterService ordersDeleterService, 
        IOrdersGetterService ordersGetterService, 
        IOrdersUpdaterService ordersUpdaterService)
    {
        _logger = logger;
        _ordersAdderService = ordersAdderService;
        _ordersDeleterService = ordersDeleterService;
        _ordersGetterService = ordersGetterService;
        _ordersUpdaterService = ordersUpdaterService;
    }

    [HttpGet]
    [ProducesResponseType<ActionResult<List<OrderResponse>>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OrderResponse>>> GetOrders()
    {
        return Ok(await _ordersGetterService.GetAllAsync());
    }

    [HttpGet("{orderId}")]
    [ProducesResponseType<ActionResult<OrderResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> GetOrder(Guid orderId)
    {
        var orderResponse = await _ordersGetterService.GetAsync(orderId);
        if(orderResponse == null)
        {
            _logger.LogWarning($"GetOrder - Not Found Order - ID: {orderId}");
            return NotFound();
        }
        return Ok(orderResponse);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<OrderResponse>> PostOrder(OrderAddRequest orderAddRequest)
    {
        var result = await _ordersAdderService.AddAsync(orderAddRequest);
        _logger.LogInformation($"PostOrder - Successful - ID: {result.OrderId}");
        return CreatedAtAction(nameof(GetOrder), new { orderId = result.OrderId }, result);
    }

    [HttpPut("{orderId}")]
    [ProducesResponseType<ActionResult<OrderResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderResponse>> PutOrder(Guid orderId, OrderUpdateRequest orderAddRequest)
    {
        if (orderId != orderAddRequest.OrderId)
        {
            _logger.LogWarning($"PutOrder - Failed - No Match Between the Supplied OrderId({orderId}) in the route and the OrderId in the Request Body({orderAddRequest.OrderId})");
            return BadRequest();
        }
        var result = await _ordersUpdaterService.UpdateAsync(orderAddRequest);
        if (result == null)
        {
            _logger.LogWarning($"PutOrder - Failed - Order Not Found, OrderId: {orderId}");
            return BadRequest();
        }
        _logger.LogInformation($"PutOrder - Success - ID: {orderId}");
        return Ok(result);
    }

    [HttpDelete("{orderId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<bool>> DeleteOrder(Guid orderId)
    {
        var result = await _ordersDeleterService.DeleteAsync(orderId);
        if (result)
        {
            _logger.LogInformation($"DeleteOrder - Success - ID: {orderId}");
            return NoContent();
        }
        _logger.LogInformation($"DeleteOrder - Failed - ID: {orderId}");
        return NotFound();
    }
}
