using OrderManagement.Core.ServiceContracts.OrderItems;
using OrderManagement.Core.Domain.RepositoryContracts;
using OrderManagement.Core.Services.OrderItems;
using OrderManagement.Core.Domain.Entities;
using Microsoft.Extensions.Logging;
using OrderManagement.Core.DTO;
using FluentAssertions;
using AutoFixture;
using Moq;

namespace OrderManagement.Test.Services;

public class Test_OrderItemsAdderService
{
    private readonly Mock<IOrderItemsRepository> _orderItemsRepoMock;
    private readonly IOrderItemsRepository _orderItemsRepo;

    private readonly IOrderItemsAdderService _adderService;
    private readonly IFixture _fixture;

    public Test_OrderItemsAdderService()
    {
        _orderItemsRepoMock = new Mock<IOrderItemsRepository>();
        _orderItemsRepo = _orderItemsRepoMock.Object;

        _adderService = new OrderItemsAdderService(_orderItemsRepo, new Mock<ILogger<OrderItemsAdderService>>().Object);

        _fixture = new Fixture();
    }

    [Fact]
    public async Task Create_Success()
    {
        // Arrange
        var orderItem = _fixture.Build<OrderItem>().Without(oi => oi.Order).Create();
        _orderItemsRepoMock.Setup(x => x.CreateAsync(It.IsAny<OrderItem>())).ReturnsAsync(orderItem);

        // Act
        var result = await _adderService.AddAsync(orderItem.ToOrderItemAddRequest());

        // Assert
        result.Should().BeEquivalentTo(orderItem.ToOrderItemResponse());
    }
}
