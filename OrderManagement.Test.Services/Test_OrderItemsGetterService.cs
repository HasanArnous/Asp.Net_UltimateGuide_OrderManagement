using OrderManagement.Core.ServiceContracts.OrderItems;
using OrderManagement.Core.Domain.RepositoryContracts;
using OrderManagement.Core.Services.OrderItems;
using OrderManagement.Core.Domain.Entities;
using System.Diagnostics.Metrics;
using OrderManagement.Core.DTO;
using FluentAssertions;
using AutoFixture;
using Moq;

namespace OrderManagement.Test.Services;

public class Test_OrderItemsGetterService
{
    private readonly Mock<IOrderItemsRepository> _orderItemsRepoMock;

    private readonly IOrderItemsRepository _orderItemsRepo;
    private readonly IOrderItemsGetterService _getterService;

    private readonly IFixture _fixture;

    public Test_OrderItemsGetterService()
    {
        _orderItemsRepoMock = new Mock<IOrderItemsRepository>();
        _orderItemsRepo = _orderItemsRepoMock.Object;

        _getterService = new OrderItemsGetterService(_orderItemsRepo);

        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetAll_EmptyTable_EmptyList()
    {
        // Arrange
        _orderItemsRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<OrderItem>());

        // Act
        var result = await _getterService.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_FilledTable ()
    {
        // Arrange
        var orderItems = new List<OrderItem>();
        for (int i = 0; i < 5; i++)
            orderItems.Add(_fixture.Build<OrderItem>().Without(oi => oi.Order).Create());
        _orderItemsRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(orderItems);

        // Act
        var result = await _getterService.GetAllAsync();

        // Assert
        result.Should().HaveCount(orderItems.Count);
        result.Should().BeEquivalentTo(orderItems.Select(oi => oi.ToOrderItemResponse()).ToList());
    }

    [Fact]
    public async Task Get_ById_Found()
    {
        // Arrange
        var expected = _fixture.Build<OrderItem>().Without(oi => oi.Order).Create();
        _orderItemsRepoMock.Setup(x => x.GetAsyncById(It.IsAny<Guid>())).ReturnsAsync(expected);

        // Act
        var result = await _getterService.GetAsyncByOrderItemId(expected.OrderId);

        // Assert
        result.Should().BeEquivalentTo(expected.ToOrderItemResponse());
    }

    [Fact]
    public async Task Get_ById_NotFound()
    {
        // Arrange
        _orderItemsRepoMock.Setup(x => x.GetAsyncById(It.IsAny<Guid>())).ReturnsAsync(null as OrderItem);

        // Act
        var result = await _getterService.GetAsyncByOrderItemId(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }


    [Fact]
    public async Task Get_ByOrderId_NotFound_ShouldReturnEmptyList()
    {
        // Arrange
        _orderItemsRepoMock.Setup(x => x.GetAsyncByOrderId(It.IsAny<Guid>())).ReturnsAsync(new List<OrderItem>());

        // Act
        var result = await _getterService.GetAsyncByOrderId(Guid.NewGuid());

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Get_ByOrderId_Founc_ShouldReturnListWithItems()
    {
        // Arrange
        var orderItems = new List<OrderItem>();
        for (int i = 0; i < 10; i++)
            orderItems.Add(_fixture.Build<OrderItem>().Without(oi => oi.Order).Create());
        _orderItemsRepoMock.Setup(x => x.GetAsyncByOrderId(It.IsAny<Guid>())).ReturnsAsync(orderItems);

        // Act
        var result = await _getterService.GetAsyncByOrderId(Guid.NewGuid());

        // Assert
        result.Should().BeEquivalentTo(orderItems.Select(oi => oi.ToOrderItemResponse()).ToList());
    }
}
