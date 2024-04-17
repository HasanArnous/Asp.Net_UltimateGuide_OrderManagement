using OrderManagement.Core.Domain.RepositoryContracts;
using OrderManagement.Core.ServiceContracts.Orders;
using OrderManagement.Core.Services.Orders;
using OrderManagement.Core.Domain.Entities;
using Microsoft.Extensions.Logging;
using OrderManagement.Core.DTO;
using FluentAssertions;
using AutoFixture;
using Moq;

namespace OrderManagement.Test.Services;

public class Test_OrdersAdderService
{
    private readonly Mock<IOrdersRepository> _ordersRepoMock;
    private readonly Mock<IOrderItemsRepository> _orderItemsRepoMock;

    private readonly IOrdersRepository _ordersRepo;
    private readonly IOrderItemsRepository _orderItemsRepo;

    private readonly IOrdersAdderService _adderService;

    private readonly IFixture _fixture;

    public Test_OrdersAdderService()
    {
        _ordersRepoMock = new Mock<IOrdersRepository>();
        _orderItemsRepoMock = new Mock<IOrderItemsRepository>();

        _ordersRepo = _ordersRepoMock.Object;
        _orderItemsRepo = _orderItemsRepoMock.Object;

        _adderService = new OrdersAdderService(_ordersRepo, _orderItemsRepo,  new Mock<ILogger<OrdersAdderService>>().Object);

        _fixture = new Fixture();
    }

    [Fact]
    public async Task Create_Success()
    {
        // Arrange
        var order = _fixture.Build<Order>().Without(o => o.OrderItems).Create();
        var expected = new Order
        {
            TotalAmount = order.TotalAmount,
            CustomerName = order.CustomerName,
            OrderDate = order.OrderDate,
            OrderId = order.OrderId,
            OrderItems = new List<OrderItem>(),
            OrderNumber = order.OrderNumber,
            SequentialNumber = order.SequentialNumber,
        };
        _ordersRepoMock.Setup(x => x.CreateAsync(It.IsAny<Order>())).ReturnsAsync(order);
        for(int i = 0; i < 3; i++)
            expected.OrderItems.Add(_fixture.Build<OrderItem>()
                .With(oi => oi.OrderId, order.OrderId)
                .With(oi => oi.Quantity, 2)
                .With(oi => oi.UnitPrice, 233)
                .With(oi => oi.TotalPrice, 233 * 2)
                .Without(oi => oi.Order).Create());
        _orderItemsRepoMock.Setup(x => x.CreateAsync(It.IsAny<OrderItem>())).ReturnsAsync((OrderItem oi) => oi);

        // Act
        var result = await _adderService.AddAsync(expected.ToOrderAddRequest());

        // Assert
        result.Should().BeEquivalentTo(expected.ToOrderResponse());
    }
}
