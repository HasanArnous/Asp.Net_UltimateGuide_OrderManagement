using OrderManagement.Core.Domain.RepositoryContracts;
using OrderManagement.Infrastructure.Repositories;
using OrderManagement.Infrastructure.DataStore;
using OrderManagement.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EntityFrameworkCoreMock;
using FluentAssertions;
using AutoFixture;
using Moq;

namespace OrderManagement.Test.Repositories;

public class UnitTest_OrdersRepository
{
    private readonly AppDbContext _db;
    private readonly DbContextMock<AppDbContext> _dbMock;
    private readonly IOrdersRepository _ordersRepository;
    private readonly IFixture _fixture;

    private readonly DbSetMock<Order> _ordersMock;
    private readonly DbSetMock<OrderItem> _orderItemsMock;
    public UnitTest_OrdersRepository()
    {
        _dbMock = new DbContextMock<AppDbContext>(new DbContextOptionsBuilder<AppDbContext>().Options);
        _ordersMock = _dbMock.CreateDbSetMock(db => db.Orders, new List<Order>());
        _orderItemsMock = _dbMock.CreateDbSetMock(db => db.OrderItems, new List<OrderItem>());
        _db = _dbMock.Object;
        _ordersRepository = new OrdersRepository(_db, new Mock<ILogger<OrdersRepository>>().Object);
        _fixture = new Fixture();
    }

    #region Create
    [Fact]
    public async Task Create_Success()
    {
        var order = _fixture.Build<Order>()
            .Without(o => o.SequentialNumber)
            .Without(o => o.OrderNumber)
            .Without(o => o.OrderItems)
            .Without(o => o.OrderDate)
            .Without(o => o.OrderId)
            .Create();
        var createdOrder = await _ordersRepository.CreateAsync(order);
        createdOrder.Should().NotBeNull();
        createdOrder.OrderId.Should().NotBeEmpty();
        createdOrder.SequentialNumber.Should().Be(1);
        createdOrder.OrderNumber.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region Getters
    [Fact]
    public async Task Get_All_EmptyList()
    {
        var orders = await _ordersRepository.GetAllAsync();
        orders.Should().BeEmpty();
    }

    [Fact]
    public async Task Get_All_List_Success()
    {
        // Arrange=
        for(int i=0; i<4; i++)
            _ordersMock.Object.Add(_fixture.Build<Order>().Without(o => o.OrderItems).Create());
        
        // Act
        var result = await _ordersRepository.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(_ordersMock.Object);
    }

    [Fact]
    public async Task Get_OrderById_Found()
    {
        // Arrange
        var order = _fixture.Build<Order>().Without(o => o.OrderItems).Create();
        _ordersMock.Object.Add(order);
        for (int i = 0; i < 4; i++)
            _ordersMock.Object.Add(_fixture.Build<Order>().Without(o => o.OrderItems).Create());

        // Act
        var existed = await _ordersRepository.GetAsync(order.OrderId);

        // Assert
        existed.Should().BeEquivalentTo(existed);
    }

    [Fact]
    public async Task Get_OrderById_NotFound()
    {
        // Arrange
        for (int i = 0; i < 4; i++)
            _ordersMock.Object.Add(_fixture.Build<Order>().Without(o => o.OrderItems).Create());

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
        var updateOrder = _fixture.Build<Order>().Without(o => o.OrderItems).Create();

        _ordersMock.Setup(o => o.FindAsync(It.IsAny<Guid>())).ReturnsAsync(null as Order);

        // Act
        var updatedOrder = await _ordersRepository.UpdateAsync(updateOrder);

        // Assert
        order.OrderId.Should().NotBe(updatedOrder.OrderId);
    }

    [Fact]
    public async Task Update_Found_AndSuccess()
    {
        // Arrange
        var order = _fixture.Build<Order>().Without(o => o.OrderItems).Create();
        var updateOrder = _fixture.Build<Order>()
            .Without(o => o.OrderItems)
            .With(o => o.OrderId, order.OrderId)
            .Create();
        _ordersMock.Setup(o => o.FindAsync(It.IsAny<Guid>())).ReturnsAsync(order);

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
        // Arrange
        _ordersMock.Setup(o => o.FindAsync(It.IsAny<Guid>())).ReturnsAsync(null as Order);

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
        _ordersMock.Setup(o => o.FindAsync(It.IsAny<Guid>())).ReturnsAsync(order);

        // Act
        var deleteResult = await _ordersRepository.DeleteAsync(order.OrderId);

        // Assert
        deleteResult.Should().BeTrue();
    }
    #endregion
}