using OrderManagement.Core.Domain.RepositoryContracts;
using OrderManagement.Core.ServiceContracts.Orders;
using OrderManagement.Core.Domain.Entities;
using OrderManagement.Core.Services.Orders;
using System.Diagnostics.Metrics;
using OrderManagement.Core.DTO;
using FluentAssertions;
using AutoFixture;
using Moq;

namespace OrderManagement.Test.Services;

public class Test_OrdersGetterService
{
    private readonly Mock<IOrdersRepository> _ordersRepoMock;

    private readonly IOrdersRepository _ordersRepo;
    private readonly IOrdersGetterService _getterService;

    private readonly IFixture _fixture;

    public Test_OrdersGetterService()
    {
        _ordersRepoMock = new Mock<IOrdersRepository>();
        _ordersRepo = _ordersRepoMock.Object;

        _getterService = new OrdersGetterService(_ordersRepo);

        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetAll_EmptyTable_EmptyList()
    {
        // Arrange
        _ordersRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Order>());

        // Act
        var result = await _getterService.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_FilledTable ()
    {
        // Arrange
        var orders = new List<Order>();
        for (int i = 0; i < 5; i++)
            orders.Add(_fixture.Build<Order>().Without(o => o.OrderItems).Create());
        _ordersRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(orders);

        // Act
        var result = await _getterService.GetAllAsync();

        // Assert
        result.Should().HaveCount(orders.Count);
        result.Should().BeEquivalentTo(orders.Select(o => o.ToOrderResponse()).ToList());
    }

    [Fact]
    public async Task Get_ById_Found()
    {
        // Arrange
        var expected = _fixture.Build<Order>().Without(o => o.OrderItems).Create();
        _ordersRepoMock.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(expected);

        // Act
        var result = await _getterService.GetAsync(expected.OrderId);

        // Assert
        result.Should().BeEquivalentTo(expected.ToOrderResponse());
    }

    [Fact]
    public async Task Get_ById_NotFound()
    {
        // Arrange
        _ordersRepoMock.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(null as Order);

        // Act
        var result = await _getterService.GetAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }
}
