using OrderManagement.Core.ServiceContracts.Orders;
using OrderManagement.Core.Domain.Entities;
using OrderManagement.WebAPI.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Core.DTO;
using FluentAssertions;
using AutoFixture;
using Moq;

namespace OrderManagement.Test.Controllers;

public class Test_OrderController
{
    private readonly Mock<IOrdersAdderService> _ordersAdderMock;
    private readonly Mock<IOrdersUpdaterService> _ordersUpdaterMock;
    private readonly Mock<IOrdersDeleterService> _ordersDeleterMock;
    private readonly Mock<IOrdersGetterService> _ordersGetterMock;

    private readonly IFixture _fixture;

    private readonly IOrdersAdderService _ordersAdder;
    private readonly IOrdersUpdaterService _ordersUpdater;
    private readonly IOrdersDeleterService _ordersDeleter;
    private readonly IOrdersGetterService _ordersGetter;

    public Test_OrderController()
    {
        _ordersAdderMock = new Mock<IOrdersAdderService>();
        _ordersUpdaterMock = new Mock<IOrdersUpdaterService>();
        _ordersDeleterMock = new Mock<IOrdersDeleterService>();
        _ordersGetterMock = new Mock<IOrdersGetterService>();

        _fixture = new Fixture();

        _ordersAdder = _ordersAdderMock.Object;
        _ordersUpdater = _ordersUpdaterMock.Object;
        _ordersDeleter = _ordersDeleterMock.Object;
        _ordersGetter = _ordersGetterMock.Object;
    }

    #region GetAll
    [Fact]
    public async Task GetAll_EmptyTable_ShouldReturnsEmptyList()
    {
        // Arrange
        var ordersController = GetOrdersController();
        _ordersGetterMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<OrderResponse>());

        // Act
        var response = await ordersController.GetOrders();

        // Assert
        var result = Assert.IsType<OkObjectResult>(response.Result);
        var ordersList = Assert.IsType<List<OrderResponse>>(result.Value);
        ordersList.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_FilledTable_ShouldReturnsTableWithData()
    {
        // Arrange
        var ordersController = GetOrdersController();
        var expected = new List<OrderResponse>();
        for(int i = 0; i < 10; i++)
            expected.Add(_fixture.Build<OrderResponse>().Without(o => o.OrderItems).Create());
        _ordersGetterMock.Setup(x => x.GetAllAsync()).ReturnsAsync(expected);

        // Act
        var response = await ordersController.GetOrders();

        // Assert
        var result = Assert.IsType<OkObjectResult>(response.Result);
        result.Value.Should().BeAssignableTo<List<OrderResponse>>();
        result.Value.Should().BeEquivalentTo(expected);
    }

    #endregion

    #region GetOrder
    [Fact]
    public async Task GetOrder_NotExist_ShouldReturnsNotFoundResult()
    {
        // Arrange
        var ordersController = GetOrdersController();
        _ordersGetterMock.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(null as OrderResponse);

        // Act
        var response = await ordersController.GetOrder(Guid.NewGuid());

        // Assert
        var result = Assert.IsType<NotFoundResult>(response.Result);
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task GetOrder_Exist_ShouldReturnsActionResultWithOrderResponse()
    {
        // Arrange
        var ordersController = GetOrdersController();
        var order = _fixture.Build<OrderResponse>().Without(o => o.OrderItems).Create();
        _ordersGetterMock.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(order);

        // Act
        var response = await ordersController.GetOrder(order.OrderId);

        // Assert
        var result = Assert.IsType<OkObjectResult>(response.Result);
        result.Value.Should().BeEquivalentTo(order);
    }
    #endregion

    #region PostOrder
    [Fact]
    public async Task PostOrder_Success_ShouldReturnsActionResultWithOrderResponse()
    {
        // Arrange
        var ordersController = GetOrdersController();
        var order = _fixture.Build<Order>().Without(o => o.OrderItems).Create();
        _ordersAdderMock.Setup(x => x.AddAsync(It.IsAny<OrderAddRequest>())).ReturnsAsync(order.ToOrderResponse());

        // Act
        var response = await ordersController.PostOrder(order.ToOrderAddRequest());

        // Assert
        var result = Assert.IsType<CreatedAtActionResult>(response.Result);
        result.Value.Should().BeEquivalentTo(order.ToOrderResponse());
    }
    #endregion

    #region PutOrder
    [Fact]
    public async Task PutOrder_Failed_OrderIdInRouteDifferentThanOrderId_ShouldReturnsBadRequest()
    {
        // Arrange
        var ordersController = GetOrdersController();
        var order = _fixture.Build<OrderUpdateRequest>().Without(o => o.OrderItems).Create();

        // Act
        var response = await ordersController.PutOrder(Guid.NewGuid(), order);

        // Assert
        var result = Assert.IsType<BadRequestResult>(response.Result);
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task PutOrder_Failed_OrderNotFound_ShouldReturnsBadRequest()
    {
        // Arrange
        var ordersController = GetOrdersController();
        var order = _fixture.Build<OrderUpdateRequest>().Without(o => o.OrderItems).Create();
        _ordersUpdaterMock.Setup(x => x.UpdateAsync(It.IsAny<OrderUpdateRequest>())).ReturnsAsync(null as OrderResponse);

        // Act
        var response = await ordersController.PutOrder(order.OrderId, order);

        // Assert
        var result = Assert.IsType<BadRequestResult>(response.Result);
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task PutOrder_Success_ShouldReturnsUpdatedOrderResponse()
    {
        // Arrange
        var ordersController = GetOrdersController();
        var order = _fixture.Build<Order>().Without(o => o.OrderItems).Create();
        _ordersUpdaterMock.Setup(x => x.UpdateAsync(It.IsAny<OrderUpdateRequest>())).ReturnsAsync(order.ToOrderResponse());

        // Act
        var response = await ordersController.PutOrder(order.OrderId, order.ToOrderUpdateRequest());

        var result = Assert.IsType<OkObjectResult>(response.Result);
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(order.ToOrderResponse());
    }

    #endregion

    #region DeleteOrder
    [Fact]
    public async Task DeleteOrder_Failed_OrderNotFound_ShouldReturnsBadRequest()
    {
        // Arrange
        var ordersController = GetOrdersController();
        var order = _fixture.Build<OrderUpdateRequest>().Without(o => o.OrderItems).Create();
        _ordersDeleterMock.Setup(x => x.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(false);

        // Act
        var response = await ordersController.DeleteOrder(order.OrderId);

        // Assert
        var result = Assert.IsType<NotFoundResult>(response.Result);
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }


    [Fact]
    public async Task DeleteOrder_Success_ShouldReturnsNoContent()
    {
        // Arrange
        var ordersController = GetOrdersController();
        var order = _fixture.Build<Order>().Without(o => o.OrderItems).Create();
        _ordersDeleterMock.Setup(x => x.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(true);

        // Act
        var response = await ordersController.DeleteOrder(order.OrderId);

        // Assert
        var result = Assert.IsType<NoContentResult>(response.Result);
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }
    #endregion

    private OrdersController GetOrdersController()
    {
        return new OrdersController(new Mock<ILogger<OrdersController>>().Object, _ordersAdder, _ordersDeleter, _ordersGetter, _ordersUpdater);
    }
}
