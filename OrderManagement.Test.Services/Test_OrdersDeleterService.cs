using OrderManagement.Core.Domain.RepositoryContracts;
using OrderManagement.Core.ServiceContracts.Orders;
using OrderManagement.Core.Domain.Entities;
using OrderManagement.Core.Services.Orders;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using AutoFixture;
using Moq;

namespace OrderManagement.Test.Services;

public class Test_OrdersDeleterService
{
    private readonly Mock<IOrdersRepository> _ordersRepoMock;

    private readonly IOrdersRepository _ordersRepo;

    private readonly IOrdersDeleterService _deleterService;

    private readonly IFixture _fixture;

    public Test_OrdersDeleterService()
    {
        _ordersRepoMock = new Mock<IOrdersRepository>();
        _ordersRepo = _ordersRepoMock.Object;

        _deleterService = new OrdersDeleterService(_ordersRepo, new Mock<ILogger<OrdersDeleterService>>().Object);

        _fixture = new Fixture();
    }

    [Fact]
    public async Task Delete_Success()
    {
        // Arrange
        var order = _fixture.Build<Order>().Without(o => o.OrderItems).Create();
        _ordersRepoMock.Setup(o => o.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(true);

        // Act
        var result = await _deleterService.DeleteAsync(order.OrderId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_Failed_NotFound()
    {
        // Arrange
        var order = _fixture.Build<Order>().Without(o => o.OrderItems).Create();
        _ordersRepoMock.Setup(o => o.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(false);

        // Act
        var result = await _deleterService.DeleteAsync(order.OrderId);

        // Assert
        result.Should().BeFalse();
    }
}
