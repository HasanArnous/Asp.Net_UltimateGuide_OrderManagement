using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrderManagement.Core.Domain.Entities;
using OrderManagement.Core.Domain.RepositoryContracts;
using OrderManagement.Core.DTO;
using OrderManagement.Core.ServiceContracts.Orders;
using OrderManagement.Core.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Test.Services;

public class Test_OrdersUpdaterService
{
    private readonly Mock<IOrdersRepository> _ordersRepoMock;

    private readonly IOrdersRepository _ordersRepo;
    private readonly IOrdersUpdaterService _updaterService;

    private readonly IFixture _fixture;

    public Test_OrdersUpdaterService()
    {
        _ordersRepoMock = new Mock<IOrdersRepository>();
        _ordersRepo = _ordersRepoMock.Object;

        _updaterService = new OrdersUpdaterService(_ordersRepo, new Mock<ILogger<OrdersUpdaterService>>().Object);

        _fixture = new Fixture();
    }

    [Fact]
    public async Task Update_FailedNotFound_ShouldReturnNull()
    {
        // Arrange
        var order = _fixture.Build<Order>().Without(o => o.OrderItems).Create();
        _ordersRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Order>())).ReturnsAsync(null as Order);

        // Act
        var updatedOrder = await _updaterService.UpdateAsync(order.ToOrderUpdateRequest());

        // Assert
        updatedOrder.Should().BeNull();
    }

    [Fact]
    public async Task Update_Success_ShouldReturnTheUpdatedOrder()
    {
        // Arrange
        var order = _fixture.Build<Order>().Without(o => o.OrderItems).Create();
        _ordersRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Order>())).ReturnsAsync(order);

        // Act
        var updatedOrderResponse = await _updaterService.UpdateAsync(order.ToOrderUpdateRequest());

        // Assert
        updatedOrderResponse.Should().NotBeNull();
        updatedOrderResponse.Should().BeEquivalentTo(order.ToOrderResponse());
    }
}
