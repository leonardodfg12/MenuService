using FluentAssertions;
using MassTransit;
using MenuService.Application.Commands;
using MenuService.Application.Contracts;
using MenuService.Application.Handlers;
using MenuService.Domain.Entities;
using MenuService.Domain.Interfaces;
using Moq;

namespace MenuService.Application.Test.Handlers;

public class MenuServiceHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldAddMenuItemAndSendEvent()
    {
        // Arrange
        var command = new CreateMenuItemCommand(
            Name: "X-Burguer",
            Description: "Delicioso lanche com queijo",
            Price: 19.99M,
            IsAvailable: true
        );

        var repositoryMock = new Mock<IMenuRepository>();
        var endpointMock = new Mock<ISendEndpoint>();
        var sendProviderMock = new Mock<ISendEndpointProvider>();

        sendProviderMock
            .Setup(x => x.GetSendEndpoint(It.IsAny<Uri>()))
            .ReturnsAsync(endpointMock.Object);

        var handler = new MenuServiceHandler(repositoryMock.Object, sendProviderMock.Object);

        // Act
        await handler.Handle(command);

        // Assert
        repositoryMock.Verify(r => r.AddAsync(It.Is<MenuItem>(m =>
            m.Name == command.Name &&
            m.Description == command.Description &&
            m.Price == command.Price &&
            m.IsAvailable == command.IsAvailable
        )), Times.Once);

        endpointMock.Verify(e => e.Send(
            It.Is<MenuItemCreatedEvent>(evt =>
                evt.Name == command.Name &&
                evt.Description == command.Description &&
                evt.Price == command.Price &&
                evt.IsAvailable == command.IsAvailable
            ),
            default
        ), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllMenuItems()
    {
        // Arrange
        var expectedItems = new List<MenuItem>
        {
            new("X-Salada", "Com alface", 18.90M, true),
            new("X-Bacon", "Com bacon crocante", 21.50M, true)
        };

        var repoMock = new Mock<IMenuRepository>();
        repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(expectedItems);

        var handler = new MenuServiceHandler(repoMock.Object, Mock.Of<ISendEndpointProvider>());

        // Act
        var result = await handler.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedItems);
        repoMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnItemById_WhenExists()
    {
        // Arrange
        var menuItem = new MenuItem("X-Frango", "Com frango empanado", 20M, true);
        var repoMock = new Mock<IMenuRepository>();
        repoMock.Setup(r => r.GetByIdAsync(menuItem.Id.ToString())).ReturnsAsync(menuItem);

        var handler = new MenuServiceHandler(repoMock.Object, Mock.Of<ISendEndpointProvider>());

        // Act
        var result = await handler.GetByIdAsync(menuItem.Id.ToString());

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(menuItem);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateMenuItem_WhenItemExists()
    {
        // Arrange
        var existingItem = new MenuItem("X-Veggie", "Com legumes", 17M, true);
        var command = new CreateMenuItemCommand("X-Veggie Atualizado", "Agora com tofu", 18M, false);

        var repoMock = new Mock<IMenuRepository>();
        repoMock.Setup(r => r.GetByIdAsync(existingItem.Id.ToString())).ReturnsAsync(existingItem);

        var handler = new MenuServiceHandler(repoMock.Object, Mock.Of<ISendEndpointProvider>());

        // Act
        await handler.UpdateAsync(existingItem.Id.ToString(), command);

        // Assert
        existingItem.Name.Should().Be(command.Name);
        existingItem.Description.Should().Be(command.Description);
        existingItem.Price.Should().Be(command.Price);
        existingItem.IsAvailable.Should().Be(command.IsAvailable);

        repoMock.Verify(r => r.UpdateAsync(existingItem), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallRepository_WhenItemIsDeleted()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var repoMock = new Mock<IMenuRepository>();
        var handler = new MenuServiceHandler(repoMock.Object, Mock.Of<ISendEndpointProvider>());

        // Act
        await handler.DeleteAsync(id);

        // Assert
        repoMock.Verify(r => r.DeleteAsync(id), Times.Once);
    }
}