using OrderManagement.Core.ServiceContracts.OrderItems;
using OrderManagement.Core.Domain.RepositoryContracts;
using OrderManagement.Core.Services.OrderItems;
using OrderManagement.Core.Services.Orders;
using OrderManagement.Core.Domain.Entities;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using AutoFixture;
using Moq;

namespace OrderManagement.Test.Services;

public class Test_OrderItemsDeleterService
{
    private readonly Mock<IOrderItemsRepository> _orderItemsRepoMock;
    private readonly IOrderItemsRepository _orderItemsRepo;

    private readonly IOrderItemsDeleterService _deleterService;
    private readonly IFixture _fixture;

    public Test_OrderItemsDeleterService()
    {
        _orderItemsRepoMock = new Mock<IOrderItemsRepository>();
        _orderItemsRepo = _orderItemsRepoMock.Object;

        _deleterService = new OrderItemsDeleterService(_orderItemsRepo, new Mock<ILogger<OrderItemsDeleterService>>().Object);

        _fixture = new Fixture();
    }

    [Fact]
    public async Task Delete_Success()
    {
        // Arrange
        var orderItem = _fixture.Build<OrderItem>().Without(oi => oi.Order).Create();
        _orderItemsRepoMock.Setup(oi => oi.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(true);

        // Act
        var result = await _deleterService.DeleteAsync(orderItem.OrderItemId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_Failed_NotFound()
    {
        // Arrange
        var orderItem = _fixture.Build<OrderItem>().Without(oi => oi.Order).Create();
        _orderItemsRepoMock.Setup(oi => oi.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(false);

        // Act
        var result = await _deleterService.DeleteAsync(orderItem.OrderItemId);

        // Assert
        result.Should().BeFalse();
    }
}
