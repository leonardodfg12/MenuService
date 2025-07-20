using MenuService.Domain.Entities;
using MenuService.Infrastructure.Persistence;
using MongoDB.Driver;
using Moq;

namespace MenuService.Infrastructure.Test.Persistence;

public class MongoMenuRepositoryTest
{
    [Fact]
    public async Task AddAsync_ShouldCallInsertOneAsync()
    {
        // Arrange
        var item = new MenuItem("X-Teste", "Descrição", 12.99m, true);

        var collectionMock = new Mock<IMongoCollection<MenuItem>>();
        collectionMock
            .Setup(c => c.InsertOneAsync(item, null, default))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var dbMock = new Mock<IMongoDatabase>();
        dbMock.Setup(db => db.GetCollection<MenuItem>("MenuItems", null))
            .Returns(collectionMock.Object);

        var repo = new MongoMenuRepository(dbMock.Object);

        // Act
        await repo.AddAsync(item);

        // Assert
        collectionMock.Verify(c => c.InsertOneAsync(item, null, default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallDeleteOneAsync()
    {
        // Arrange
        var id = "123";
        var filterCaptured = default(FilterDefinition<MenuItem>);

        var collectionMock = new Mock<IMongoCollection<MenuItem>>();
        collectionMock
            .Setup(c => c.DeleteOneAsync(
                It.IsAny<FilterDefinition<MenuItem>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<DeleteResult>())
            .Verifiable();

        var dbMock = new Mock<IMongoDatabase>();
        dbMock.Setup(db => db.GetCollection<MenuItem>("MenuItems", null))
            .Returns(collectionMock.Object);

        var repo = new MongoMenuRepository(dbMock.Object);

        // Act
        await repo.DeleteAsync(id);

        // Assert
        collectionMock.Verify(c =>
                c.DeleteOneAsync(It.IsAny<FilterDefinition<MenuItem>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldCallReplaceOneAsync()
    {
        // Arrange
        var item = new MenuItem("X-Atualizado", "Desc", 30m, true);
        var collectionMock = new Mock<IMongoCollection<MenuItem>>();
        var dbMock = new Mock<IMongoDatabase>();

        dbMock.Setup(db => db.GetCollection<MenuItem>("MenuItems", null))
            .Returns(collectionMock.Object);

        var repo = new MongoMenuRepository(dbMock.Object);

        collectionMock
            .Setup(c => c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<MenuItem>>(),
                item,
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<ReplaceOneResult>())
            .Verifiable();

        // Act
        await repo.UpdateAsync(item);

        // Assert
        collectionMock.Verify(c =>
                c.ReplaceOneAsync(It.IsAny<FilterDefinition<MenuItem>>(), item, It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }
}