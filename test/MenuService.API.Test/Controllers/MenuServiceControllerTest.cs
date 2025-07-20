using FluentAssertions;
using MassTransit;
using MenuService.API.Controllers;
using MenuService.Application.Commands;
using MenuService.Application.Contracts;
using MenuService.Application.Handlers;
using MenuService.Domain.Entities;
using MenuService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MenuService.API.Test.Controllers;

public class MenuServiceControllerTest
{
    [Fact]
    public async Task Create_ShouldReturnOkResult()
    {
        // Arrange
        var command = new CreateMenuItemCommand("X", "desc", 10, true);
        var repoMock = new Mock<IMenuRepository>();
        var endpointMock = new Mock<ISendEndpoint>();
        var sendProviderMock = new Mock<ISendEndpointProvider>();

        sendProviderMock
            .Setup(x => x.GetSendEndpoint(It.IsAny<Uri>()))
            .ReturnsAsync(endpointMock.Object);

        var handler = new MenuServiceHandler(repoMock.Object, sendProviderMock.Object);
        var controller = new MenuServiceController(handler);

        // Act
        var result = await controller.Create(command) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(200);
        repoMock.Verify(r => r.AddAsync(It.IsAny<MenuItem>()), Times.Once);
        endpointMock.Verify(e => e.Send(It.IsAny<MenuItemCreatedEvent>(), default), Times.Once);
    }

    [Fact]
    public async Task GetAll_ShouldReturnListOfItems()
    {
        var expected = new List<MenuItem> { new("X", "d", 10, true) };
        var repoMock = new Mock<IMenuRepository>();
        repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(expected);

        var handler = new MenuServiceHandler(repoMock.Object, Mock.Of<ISendEndpointProvider>());
        var controller = new MenuServiceController(handler);

        var result = await controller.GetAll();

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenNull()
    {
        var repoMock = new Mock<IMenuRepository>();
        repoMock.Setup(r => r.GetByIdAsync("xyz")).ReturnsAsync((MenuItem?)null);

        var handler = new MenuServiceHandler(repoMock.Object, Mock.Of<ISendEndpointProvider>());
        var controller = new MenuServiceController(handler);

        var result = await controller.GetById("xyz");

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Update_ShouldReturnNoContent()
    {
        var command = new CreateMenuItemCommand("X", "d", 10, true);
        var existingItem = new MenuItem("Old", "Old", 5, false) { Id = "abc" };

        var repoMock = new Mock<IMenuRepository>();
        repoMock.Setup(r => r.GetByIdAsync("abc")).ReturnsAsync(existingItem);
        repoMock.Setup(r => r.UpdateAsync(It.IsAny<MenuItem>())).Returns(Task.CompletedTask);

        var handler = new MenuServiceHandler(repoMock.Object, Mock.Of<ISendEndpointProvider>());
        var controller = new MenuServiceController(handler);

        var result = await controller.Update("abc", command);

        result.Should().BeOfType<NoContentResult>();
        repoMock.Verify(r => r.UpdateAsync(It.IsAny<MenuItem>()), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent()
    {
        var repoMock = new Mock<IMenuRepository>();
        repoMock.Setup(r => r.DeleteAsync("abc")).Returns(Task.CompletedTask);

        var handler = new MenuServiceHandler(repoMock.Object, Mock.Of<ISendEndpointProvider>());
        var controller = new MenuServiceController(handler);

        var result = await controller.Delete("abc");

        result.Should().BeOfType<NoContentResult>();
        repoMock.Verify(r => r.DeleteAsync("abc"), Times.Once);
    }
}