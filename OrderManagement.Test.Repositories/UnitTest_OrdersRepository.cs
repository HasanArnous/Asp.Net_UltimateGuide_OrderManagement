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

public class UnitTest_OrdersRepository
{
    private readonly AppDbContext _db;
    private readonly IOrdersRepository _ordersRepository;
    private readonly IFixture _fixture;

    public UnitTest_OrdersRepository()
    {
        _db = new AppDbContext(new DbContextOptionsBuilder().UseInMemoryDatabase("OrderManagement_Repositories_Test_DB").Options);
        _ordersRepository = new OrdersRepository(_db, new Mock<ILogger<OrdersRepository>>().Object);
        _fixture = new Fixture();
    }

    #region Create
    [Fact]
    public async Task Create_Success()
    {
        // Arrange
        var order = _fixture.Build<Order>()
            .Without(o => o.SequentialNumber)
            .Without(o => o.OrderNumber)
            .Without(o => o.OrderItems)
            .Without(o => o.OrderDate)
            .Without(o => o.OrderId)
            .Create();

        // Act
        var createdOrder = await _ordersRepository.CreateAsync(order);
        var max = await _db.Orders.MaxAsync(o => o.SequentialNumber);

        // Assert
        createdOrder.Should().NotBeNull();
        createdOrder.OrderId.Should().NotBeEmpty();
        createdOrder.SequentialNumber.Should().Be(max);
        createdOrder.OrderNumber.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region Getters

    [Fact]
    public async Task Get_All_List_Success()
    {
        // Arrange
        var expected = new List<Order>();
        for (int i = 0; i < 4; i++)
            expected.Add(_fixture.Build<Order>().Without(o => o.OrderItems).Create());
        _db.Orders.AddRange(expected);
        await _db.SaveChangesAsync();

        // Act
        var result = await _ordersRepository.GetAllAsync();

        // Assert
        result.Should().Contain(expected);
    }

    [Fact]
    public async Task Get_OrderById_Found()
    {
        // Arrange
        var order = _fixture.Build<Order>().Without(o => o.OrderItems).Create();
        _db.Orders.Add(order);
        for (int i = 0; i < 4; i++)
            _db.Orders.Add(_fixture.Build<Order>().Without(o => o.OrderItems).Create());
        await _db.SaveChangesAsync();

        // Act
        var existed = await _ordersRepository.GetAsync(order.OrderId);

        // Assert
        existed.Should().NotBeNull();
        existed.Should().BeEquivalentTo(order);
    }

    [Fact]
    public async Task Get_OrderById_NotFound()
    {
        // Arrange
        for (int i = 0; i < 4; i++)
            _db.Orders.Add(_fixture.Build<Order>().Without(o => o.OrderItems).Create());
        await _db.SaveChangesAsync();


        // Act 
        var notExisted = await _ordersRepository.GetAsync(Guid.NewGuid());

        // Assert 
        notExisted.Should().BeNull();
    }
    #endregion

    #region Updater
    [Fact]
    public async Task Update_NotFound()
    {
        // Arrange
        var order = _fixture.Build<Order>().Without(o => o.OrderItems).Create();
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        var updateOrder = _fixture.Build<Order>().Without(o => o.OrderItems).Create();

        // Act
        var updatedOrder = await _ordersRepository.UpdateAsync(updateOrder);

        // Assert
        updatedOrder.Should().BeNull();
    }

    [Fact]
    public async Task Update_Found_AndSuccess()
    {
        // Arrange
        var order = _fixture.Build<Order>().Without(o => o.OrderItems).Create();
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        var updateOrder = _fixture.Build<Order>()
            .Without(o => o.OrderItems)
            .With(o => o.OrderId, order.OrderId)
            .Create();

        // Act
        var updatedOrder = await _ordersRepository.UpdateAsync(updateOrder);

        // Assert
        updateOrder.Should().BeEquivalentTo(updatedOrder);
    }
    #endregion

    #region Deleter
    [Fact]
    public async Task Delete_NotFound()
    {
        // Act
        var deleteResult = await _ordersRepository.DeleteAsync(Guid.NewGuid());

        // Assert
        deleteResult.Should().BeFalse();
    }

    [Fact]
    public async Task Delete_Found_AndSuccess()
    {
        // Arrange
        var order = _fixture.Build<Order>().Without(o => o.OrderItems).Create();
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        // Act
        var deleteResult = await _ordersRepository.DeleteAsync(order.OrderId);

        // Assert
        deleteResult.Should().BeTrue();
    }
    #endregion
}