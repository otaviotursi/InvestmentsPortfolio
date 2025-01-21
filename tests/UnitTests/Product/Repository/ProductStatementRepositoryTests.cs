using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Products.Repository;
using Products.Repository.Interface;
using Xunit;
using Infrastructure.Repository.Entities;

namespace UnitTests.Product.Repository
{
    public class ProductStatementRepositoryTests
    {
        private readonly Mock<IMongoCollection<BsonDocument>> _collectionMock;
        private readonly Mock<IMongoClient> _mongoClientMock;
        private readonly ProductStatementRepository _repository;

        public ProductStatementRepositoryTests()
        {
            _collectionMock = new Mock<IMongoCollection<BsonDocument>>();
            _mongoClientMock = new Mock<IMongoClient>();
            var databaseMock = new Mock<IMongoDatabase>();
            _mongoClientMock.Setup(c => c.GetDatabase(It.IsAny<string>(), null)).Returns(databaseMock.Object);
            databaseMock.Setup(d => d.GetCollection<BsonDocument>(It.IsAny<string>(), null)).Returns(_collectionMock.Object);
            _repository = new ProductStatementRepository(_mongoClientMock.Object, "TestDatabase", "TestCollection");
        }

        [Fact(DisplayName = "Should insert product statement successfully")]
        public async Task InsertAsync_Success()
        {
            // Arrange
            var product = new ProductDomain(Guid.NewGuid(), "Product1", 100, 10, "Type1", DateTime.UtcNow, 1);
            _collectionMock.Setup(c => c.InsertOneAsync(It.IsAny<BsonDocument>(), null, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            await _repository.InsertAsync(product, CancellationToken.None);

            // Assert
            _collectionMock.Verify(c => c.InsertOneAsync(It.IsAny<BsonDocument>(), null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Should update product statement successfully")]
        public async Task UpdateAsync_Success()
        {
            // Arrange
            var product = new ProductDomain(Guid.NewGuid(), "Product1", 100, 10, "Type1", DateTime.UtcNow, 1);
            _collectionMock.Setup(c => c.InsertOneAsync(It.IsAny<BsonDocument>(), null, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            await _repository.UpdateAsync(product, CancellationToken.None);

            // Assert
            _collectionMock.Verify(c => c.InsertOneAsync(It.IsAny<BsonDocument>(), null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Should delete product statement successfully")]
        public async Task DeleteAsync_Success()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _collectionMock.Setup(c => c.InsertOneAsync(It.IsAny<BsonDocument>(), null, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            await _repository.DeleteAsync(productId, CancellationToken.None);

            // Assert
            _collectionMock.Verify(c => c.InsertOneAsync(It.IsAny<BsonDocument>(), null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Should get product statements by filters successfully")]
        public async Task GetStatementBy_Success()
        {
            // Arrange
            var documents = new List<BsonDocument>
            {
                new BsonDocument
                {
                    { "Data", new BsonDocument
                        {
                            { "_id", Guid.NewGuid() },
                            { "Name", "Product1" },
                            { "UserId", 1 },
                            { "UnitPrice", "100" },
                            { "ExpirationDate", DateTime.UtcNow },
                            { "ProductType", "Type1" },
                            { "Type", "Insert" },
                            { "AvailableQuantity", "10" }
                        }
                    }
                },
                new BsonDocument
                {
                    { "Data", new BsonDocument
                        {
                            { "_id", Guid.NewGuid() },
                            { "Name", "Product2" },
                            { "UserId", 2 },
                            { "UnitPrice", "200" },
                            { "ExpirationDate", DateTime.UtcNow },
                            { "ProductType", "Type2" },
                            { "Type", "Insert" },
                            { "AvailableQuantity", "20" }
                        }
                    }
                }
            };
            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
            cursorMock.SetupGet(c => c.Current).Returns(documents);
            _collectionMock.Setup(c => c.FindSync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<FindOptions<BsonDocument, BsonDocument>>(), It.IsAny<CancellationToken>())).Returns(cursorMock.Object);

            // Act
            var result = await _repository.GetStatementBy("Product1", 1, DateTime.UtcNow, null, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }
    }
}
