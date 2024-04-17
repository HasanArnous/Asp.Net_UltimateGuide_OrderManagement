using OrderManagement.Core.Domain.RepositoryContracts;
using OrderManagement.Infrastructure.Repositories;
using OrderManagement.Infrastructure.DataStore;
using OrderManagement.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using AutoFixture;
using Moq;

namespace OrderManagement.Test.Repositories;

public class UnitTest_OrderItemsRepository
{
    private readonly AppDbContext _db;
    private readonly IOrderItemsRepository _orderItemsRepository;
    private readonly IFixture _fixture;

    public UnitTest_OrderItemsRepository()
    {
        _db = new AppDbContext(new DbContextOptionsBuilder().UseInMemoryDatabase("OrderManagement_Repositories_Test_DB").Options);
        _orderItemsRepository = new OrderItemsRepository(_db, new Mock<ILogger<OrderItemsRepository>>().Object);
        _fixture = new Fixture();
    }

    #region Create
    [Fact]
    public async Task Create_Success()
    {
        // Arrange
        var order = _fixture.Build<Order>().Without(o => o.OrderItems).Create();
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        var orderItem = _fixture.Build<OrderItem>()
            .With(oi => oi.OrderItemId, order.OrderId)
            .Without(oi => oi.Order)
            .Create();

        // Act
        var createdOrderItem = await _orderItemsRepository.CreateAsync(orderItem);

        // Assert
        createdOrderItem.Should().NotBeNull();
    }

    #endregion

    #region Delete
    [Fact]
    public async Task Delete_Failed_NotFound()
    {
        // Arrange

        // Act
        var result = await _orderItemsRepository.DeleteAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Delete_Success()
    {
        // Arrange
        var order = _fixture.Build<Order>().Without(o => o.OrderItems).Create();
        var orderItem = _fixture.Build<OrderItem>()
            .With(oi => oi.OrderId, order.OrderId)
            .Without(oi => oi.Order)
            .Create();
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        _db.OrderItems.Add(orderItem);
        await _db.SaveChangesAsync();
        //_orderItemsMock.Setup(items => items.FindAsync(orderItem.OrderItemId)).ReturnsAsync(orderItem);

        // Act
        var result = await _orderItemsRepository.DeleteAsync(orderItem.OrderItemId);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Getters
    [Fact]
    public async Task GetAll_EmptyList()
    {
        // Act
        var orderItems = await _orderItemsRepository.GetAllAsync();

        // Assert
        orderItems.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_ListSuccess()
    {
        // Arrange
        var order1 = _fixture.Build<Order>().Without(o => o.OrderItems).Create();
        var order2 = _fixture.Build<Order>().Without(o => o.OrderItems).Create();

        var item1 = _fixture.Build<OrderItem>()
            .With(oi => oi.OrderId, order1.OrderId)
            .Without(oi => oi.Order)
            .Create();
        var item2 = _fixture.Build<OrderItem>()
            .With(oi => oi.OrderId, order1.OrderId)
            .Without(oi => oi.Order)
            .Create();

        var item3 = _fixture.Build<OrderItem>()
            .With(oi => oi.OrderId, order2.OrderId)
            .Without(oi => oi.Order)
            .Create();

        _db.Orders.AddRange([order1, order2]);
        await _db.SaveChangesAsync();
        _db.OrderItems.AddRange([item1, item2, item3]);
        await _db.SaveChangesAsync();

        // Act
        var orderItems = await _orderItemsRepository.GetAllAsync();
        orderItems.Should().NotBeEmpty();
        orderItems.Should().Contain(item2);
    }

    [Fact]
    public async Task GetItem_ByItemId_Found()
    {
        // Arrange
        var order1 = _fixture.Build<Order>().Without(o => o.OrderItems).Create();
        var order2 = _fixture.Build<Order>().Without(o => o.OrderItems).Create();

        var item1 = _fixture.Build<OrderItem>()
            .With(oi => oi.OrderId, order1.OrderId)
            .Without(oi => oi.Order)
            .Create();
        var item2 = _fixture.Build<OrderItem>()
            .With(oi => oi.OrderId, order1.OrderId)
            .Without(oi => oi.Order)
            .Create();

        var item3 = _fixture.Build<OrderItem>()
            .With(oi => oi.OrderId, order2.OrderId)
            .Without(oi => oi.Order)
            .Create();

        _db.Orders.AddRange([order1, order2]);
        await _db.SaveChangesAsync();
        _db.OrderItems.AddRange([item1, item2, item3]);
        await _db.SaveChangesAsync();

        // Act
        var result = await _orderItemsRepository.GetAsyncById(item3.OrderItemId);
        result.Should().NotBeNull();
        result.Should().Be(item3);
    }

    [Fact]
    public async Task GetItem_ByItemId_NotFound()
    {
        // Arrange
        var order1 = _fixture.Build<Order>().Without(o => o.OrderItems).Create();
        var order2 = _fixture.Build<Order>().Without(o => o.OrderItems).Create();

        var item1 = _fixture.Build<OrderItem>()
            .With(oi => oi.OrderId, order1.OrderId)
            .Without(oi => oi.Order)
            .Create();
        var item2 = _fixture.Build<OrderItem>()
            .With(oi => oi.OrderId, order1.OrderId)
            .Without(oi => oi.Order)
            .Create();

        var item3 = _fixture.Build<OrderItem>()
            .With(oi => oi.OrderId, order2.OrderId)
            .Without(oi => oi.Order)
            .Create();

        _db.Orders.AddRange([order1, order2]);
        await _db.SaveChangesAsync();
        _db.OrderItems.AddRange([item1, item2, item3]);
        await _db.SaveChangesAsync();

        // Act 
        var result = await _orderItemsRepository.GetAsyncById(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetItems_ByOrderId_Found()
    {
        // Arrange
        var order1 = _fixture.Build<Order>().Without(o => o.OrderItems).Create();
        var order2 = _fixture.Build<Order>().Without(o => o.OrderItems).Create();

        var item1 = _fixture.Build<OrderItem>()
            .With(oi => oi.OrderId, order1.OrderId)
            .Without(oi => oi.Order)
            .Create();
        var item2 = _fixture.Build<OrderItem>()
            .With(oi => oi.OrderId, order1.OrderId)
            .Without(oi => oi.Order)
            .Create();

        var item3 = _fixture.Build<OrderItem>()
            .With(oi => oi.OrderId, order2.OrderId)
            .Without(oi => oi.Order)
            .Create();

        _db.Orders.AddRange([order1, order2]);
        await _db.SaveChangesAsync();
        _db.OrderItems.AddRange([item1, item2, item3]);
        await _db.SaveChangesAsync();

        // Act
        var result = await _orderItemsRepository.GetAsyncByOrderId(order1.OrderId);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(2);
        result.Should().Contain(item1);
        result.Should().Contain(item2);
    }

    [Fact]
    public async Task GetItems_ByOrderId_NotFound()
    {
        // Arrange
        var order1 = _fixture.Build<Order>().Without(o => o.OrderItems).Create();
        var order2 = _fixture.Build<Order>().Without(o => o.OrderItems).Create();

        var item1 = _fixture.Build<OrderItem>()
            .With(oi => oi.OrderId, order1.OrderId)
            .Without(oi => oi.Order)
            .Create();
        var item2 = _fixture.Build<OrderItem>()
            .With(oi => oi.OrderId, order1.OrderId)
            .Without(oi => oi.Order)
            .Create();

        var item3 = _fixture.Build<OrderItem>()
            .With(oi => oi.OrderId, order2.OrderId)
            .Without(oi => oi.Order)
            .Create();

        _db.Orders.AddRange([order1, order2]);
        await _db.SaveChangesAsync();
        _db.OrderItems.AddRange([item1, item2, item3]);
        await _db.SaveChangesAsync();

        // Act
        var result = await _orderItemsRepository.GetAsyncByOrderId(Guid.NewGuid());

        // Assert
        result.Should().BeEmpty();
    }
    #endregion

    #region Update

    [Fact]
    public async Task Update_NotFound()
    {
        // Arrange
        var order1 = _fixture.Build<Order>().Without(o => o.OrderItems).Create();
        var order2 = _fixture.Build<Order>().Without(o => o.OrderItems).Create();

        var item1 = _fixture.Build<OrderItem>()
            .With(oi => oi.OrderId, order1.OrderId)
            .Without(oi => oi.Order)
            .Create();
        var item2 = _fixture.Build<OrderItem>()
            .With(oi => oi.OrderId, order1.OrderId)
            .Without(oi => oi.Order)
            .Create();

        var item3 = _fixture.Build<OrderItem>()
            .With(oi => oi.OrderId, order2.OrderId)
            .Without(oi => oi.Order)
            .Create();

        _db.Orders.AddRange([order1, order2]);
        await _db.SaveChangesAsync();
        _db.OrderItems.AddRange([item1, item2, item3]);
        await _db.SaveChangesAsync();

        var updateItem = _fixture.Build<OrderItem>().Without(oi => oi.Order).Create();

        // Act
        var updatedItem = await _orderItemsRepository.UpdateAsync(updateItem);

        // Assert
        updatedItem.Should().BeNull();
    }

    [Fact]
    public async Task Update_Found_AndSuccess()
    {
        // Arrange
        var order1 = _fixture.Build<Order>().Without(o => o.OrderItems).Create();
        var order2 = _fixture.Build<Order>().Without(o => o.OrderItems).Create();

        var item1 = _fixture.Build<OrderItem>()
            .With(oi => oi.OrderId, order1.OrderId)
            .Without(oi => oi.Order)
            .Create();
        var item2 = _fixture.Build<OrderItem>()
            .With(oi => oi.OrderId, order1.OrderId)
            .Without(oi => oi.Order)
            .Create();

        var item3 = _fixture.Build<OrderItem>()
            .With(oi => oi.OrderId, order2.OrderId)
            .Without(oi => oi.Order)
            .Create();

        var updateItem = _fixture.Build<OrderItem>()
            .Without(oi => oi.Order)
            .With(oi => oi.OrderId, item1.OrderId)
            .With(oi => oi.OrderItemId, item1.OrderItemId)
            .Create();

        _db.Orders.AddRange([order1, order2]);
        await _db.SaveChangesAsync();
        _db.OrderItems.AddRange([item1, item2, item3]);
        await _db.SaveChangesAsync();

        // Act
        var updatedItem = await _orderItemsRepository.UpdateAsync(updateItem);
        var expected = await _db.OrderItems.FindAsync(item1.OrderItemId);

        // Assert
        updatedItem.Should().BeEquivalentTo(expected);
    }

    #endregion
}
