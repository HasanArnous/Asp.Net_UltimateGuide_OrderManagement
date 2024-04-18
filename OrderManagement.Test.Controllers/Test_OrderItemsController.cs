using OrderManagement.Core.ServiceContracts.OrderItems;
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

public class Test_OrderItemsController
{
    private readonly Mock<IOrderItemsAdderService> _orderItemsAdderMock;
    private readonly Mock<IOrderItemsUpdaterService> _orderItemsUpdaterMock;
    private readonly Mock<IOrderItemsDeleterService> _orderItemsDeleterMock;
    private readonly Mock<IOrderItemsGetterService> _orderItemsGetterMock;

    private readonly IFixture _fixture;

    private readonly IOrderItemsAdderService _orderItemsAdder;
    private readonly IOrderItemsUpdaterService _orderItemsUpdater;
    private readonly IOrderItemsDeleterService _orderItemsDeleter;
    private readonly IOrderItemsGetterService _orderItemsGetter;

    public Test_OrderItemsController()
    {
        _orderItemsAdderMock = new Mock<IOrderItemsAdderService>();
        _orderItemsUpdaterMock = new Mock<IOrderItemsUpdaterService>();
        _orderItemsDeleterMock = new Mock<IOrderItemsDeleterService>();
        _orderItemsGetterMock = new Mock<IOrderItemsGetterService>();

        _fixture = new Fixture();

        _orderItemsAdder = _orderItemsAdderMock.Object;
        _orderItemsUpdater = _orderItemsUpdaterMock.Object;
        _orderItemsDeleter = _orderItemsDeleterMock.Object;
        _orderItemsGetter = _orderItemsGetterMock.Object;
    }

    #region GetAll
    [Fact]
    public async Task GetAll_EmptyTable_ShouldReturnsEmptyList()
    {
        // Arrange
        var orderItemsController = GetOrderItemsController();
        _orderItemsGetterMock.Setup(x => x.GetAsyncByOrderId(It.IsAny<Guid>())).ReturnsAsync(new List<OrderItemResponse>());

        // Act
        var response = await orderItemsController.GetOrderItems(Guid.NewGuid());

        // Assert
        var result = Assert.IsType<OkObjectResult>(response.Result);
        var ordersList = Assert.IsType<List<OrderItemResponse>>(result.Value);
        ordersList.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_FilledTable_ShouldReturnsTableWithData()
    {
        // Arrange
        var orderItemsController = GetOrderItemsController();
        var expected = new List<OrderItemResponse>();
        for(int i = 0; i < 10; i++)
            expected.Add(_fixture.Build<OrderItemResponse>().Create());
        _orderItemsGetterMock.Setup(x => x.GetAsyncByOrderId(It.IsAny<Guid>())).ReturnsAsync(expected);

        // Act
        var response = await orderItemsController.GetOrderItems(Guid.NewGuid());

        // Assert
        var result = Assert.IsType<OkObjectResult>(response.Result);
        result.Value.Should().BeAssignableTo<List<OrderItemResponse>>();
        result.Value.Should().BeEquivalentTo(expected);
    }

    #endregion

    #region GetOrderItem
    [Fact]
    public async Task GetOrderItem_NotExist_ShouldReturnsNotFoundResult()
    {
        // Arrange
        var orderItemsController = GetOrderItemsController();
        _orderItemsGetterMock.Setup(x => x.GetAsyncByOrderItemId(It.IsAny<Guid>())).ReturnsAsync(null as OrderItemResponse);

        // Act
        var response = await orderItemsController.GetOrderItem(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        var result = Assert.IsType<NotFoundResult>(response.Result);
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task GetOrderItem_Exist_ShouldReturnsActionResultWithOrderItemResponse()
    {
        // Arrange
        var orderItemsController = GetOrderItemsController();
        var orderItem = _fixture.Build<OrderItemResponse>().Create();
        _orderItemsGetterMock.Setup(x => x.GetAsyncByOrderItemId(It.IsAny<Guid>())).ReturnsAsync(orderItem);

        // Act
        var response = await orderItemsController.GetOrderItem(orderItem.OrderId, orderItem.OrderItemId);

        // Assert
        var result = Assert.IsType<OkObjectResult>(response.Result);
        result.Value.Should().BeEquivalentTo(orderItem);
    }
    #endregion

    #region PostOrderItem
    [Fact]
    public async Task PostOrderItem_Failed_NoMatchBetweenOrderIdAndOrderIdInItem_ShouldReturnsBadRequest()
    {
        // Arrange
        var orderItemsController = GetOrderItemsController();
        var orderItem = _fixture.Build<OrderItem>().Without(oi => oi.Order).Create();
        _orderItemsAdderMock.Setup(x => x.AddAsync(It.IsAny<OrderItemAddRequest>())).ReturnsAsync(orderItem.ToOrderItemResponse());

        // Act
        var response = await orderItemsController.PostOrderItem(Guid.NewGuid(), orderItem.ToOrderItemAddRequest());

        // Assert
        var result = Assert.IsType<BadRequestResult>(response.Result);
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task PostOrderItem_Success_ShouldReturnsActionResultWithOrderItemResponse()
    {
        // Arrange
        var orderItemsController = GetOrderItemsController();
        var orderItem = _fixture.Build<OrderItem>().Without(oi => oi.Order).Create();
        _orderItemsAdderMock.Setup(x => x.AddAsync(It.IsAny<OrderItemAddRequest>())).ReturnsAsync(orderItem.ToOrderItemResponse());

        // Act
        var response = await orderItemsController.PostOrderItem(orderItem.OrderId, orderItem.ToOrderItemAddRequest());

        // Assert
        var result = Assert.IsType<CreatedAtActionResult>(response.Result);
        result.Value.Should().BeEquivalentTo(orderItem.ToOrderItemResponse());
    }
    #endregion

    #region PutOrder
    [Fact]
    public async Task PutOrderItem_Failed_OrderIdInRouteDifferentThanOrderIdInOrderItem_ShouldReturnsBadRequest()
    {
        // Arrange
        var orderItemsController = GetOrderItemsController();
        var orderItem = _fixture.Build<OrderItemUpdateRequest>().Create();

        // Act
        var response = await orderItemsController.PutOrderItem(Guid.NewGuid(), orderItem.OrderItemId, orderItem);

        // Assert
        var result = Assert.IsType<BadRequestResult>(response.Result);
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }
    public async Task PutOrderItem_Failed_OrderItemIdInRouteDifferentThanOrderItemId_ShouldReturnsBadRequest()
    {
        // Arrange
        var orderItemsController = GetOrderItemsController();
        var orderItem = _fixture.Build<OrderItemUpdateRequest>().Create();

        // Act
        var response = await orderItemsController.PutOrderItem(orderItem.OrderId, Guid.NewGuid(), orderItem);

        // Assert
        var result = Assert.IsType<BadRequestResult>(response.Result);
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task PutOrderItem_Failed_OrderNotFound_ShouldReturnsBadRequest()
    {
        // Arrange
        var orderItemsController = GetOrderItemsController();
        var orderItem = _fixture.Build<OrderItemUpdateRequest>().Create();
        _orderItemsUpdaterMock.Setup(x => x.UpdateAsync(It.IsAny<OrderItemUpdateRequest>())).ReturnsAsync(null as OrderItemResponse);

        // Act
        var response = await orderItemsController.PutOrderItem(orderItem.OrderId, orderItem.OrderItemId, orderItem);

        // Assert
        var result = Assert.IsType<BadRequestResult>(response.Result);
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task PutOrderItem_Success_ShouldReturnsUpdatedOrderResponse()
    {
        // Arrange
        var orderItemsController = GetOrderItemsController();
        var orderItem = _fixture.Build<OrderItem>().Without(oi => oi.Order).Create();
        _orderItemsUpdaterMock.Setup(x => x.UpdateAsync(It.IsAny<OrderItemUpdateRequest>())).ReturnsAsync(orderItem.ToOrderItemResponse());

        // Act
        var response = await orderItemsController.PutOrderItem(orderItem.OrderId, orderItem.OrderItemId, orderItem.ToOrderItemUpdateRequest());

        var result = Assert.IsType<OkObjectResult>(response.Result);
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(orderItem.ToOrderItemResponse());
    }

    #endregion

    #region DeleteOrder
    [Fact]
    public async Task DeleteOrderItem_Failed_OrderItemNotFound_ShouldReturnsBadRequest()
    {
        // Arrange
        var orderItemsController = GetOrderItemsController();
        var orderItem = _fixture.Build<OrderItemUpdateRequest>().Create();
        _orderItemsDeleterMock.Setup(x => x.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(false);

        // Act
        var response = await orderItemsController.DeleteOrderItem(orderItem.OrderId, orderItem.OrderItemId);

        // Assert
        var result = Assert.IsType<NotFoundResult>(response.Result);
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }


    [Fact]
    public async Task DeleteOrder_Success_ShouldReturnsNoContent()
    {
        // Arrange
        var orderItemsController = GetOrderItemsController();
        var orderItem = _fixture.Build<OrderItemUpdateRequest>().Create();
        _orderItemsDeleterMock.Setup(x => x.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(true);

        // Act
        var response = await orderItemsController.DeleteOrderItem(orderItem.OrderId, orderItem.OrderItemId);

        // Assert
        var result = Assert.IsType<NoContentResult>(response.Result);
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }
    #endregion

    private OrderItemsController GetOrderItemsController()
    {
        return new OrderItemsController(new Mock<ILogger<OrderItemsController>>().Object, _orderItemsAdder, _orderItemsDeleter, _orderItemsGetter, _orderItemsUpdater);
    }
}
