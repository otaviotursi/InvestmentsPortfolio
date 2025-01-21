//using System;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;
//using MongoDB.Driver;
//using Moq;
//using Products.Repository;
//using Products.Repository.Interface;
//using Xunit;
//using Infrastructure.Repository.Entities;

//namespace UnitTests.Product.Repository
//{
//    public class ProductRepositoryTests
//    {
//        private readonly Mock<IMongoCollection<ProductDomain>> _collectionMock;
//        private readonly Mock<IMongoClient> _mongoClientMock;
//        private readonly ProductRepository _repository;

//        public ProductRepositoryTests()
//        {
//            _collectionMock = new Mock<IMongoCollection<ProductDomain>>();
//            _mongoClientMock = new Mock<IMongoClient>();
//            var databaseMock = new Mock<IMongoDatabase>();
//            _mongoClientMock.Setup(c => c.GetDatabase(It.IsAny<string>(), null)).Returns(databaseMock.Object);
//            databaseMock.Setup(d => d.GetCollection<ProductDomain>(It.IsAny<string>(), null)).Returns(_collectionMock.Object);
//            _repository = new ProductRepository(_mongoClientMock.Object, "TestDatabase", "TestCollection");
//        }

//        [Fact(DisplayName = "Should get all products successfully")]
//        public async Task GetAll_Success()
//        {
//            // Arrange
//            var products = new List<ProductDomain>
//            {
//                new ProductDomain(Guid.NewGuid(), "Product1", 100, 10, "Type1", DateTime.UtcNow, 1),
//                new ProductDomain(Guid.NewGuid(), "Product2", 200, 20, "Type2", DateTime.UtcNow, 2)
//            };
//            var cursorMock = new Mock<IAsyncCursor<ProductDomain>>();
//            cursorMock.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
//            cursorMock.SetupGet(c => c.Current).Returns(products);
//            _collectionMock.Setup(c => c.FindSync(It.IsAny<FilterDefinition<ProductDomain>>(), It.IsAny<FindOptions<ProductDomain, ProductDomain>>(), It.IsAny<CancellationToken>())).Returns(cursorMock.Object);

//            // Act
//            var result = await _repository.GetAll(CancellationToken.None);

//            // Assert
//            Assert.Equal(products, result);
//        }

//        [Fact(DisplayName = "Should delete product successfully")]
//        public async Task DeleteAsync_Success()
//        {
//            // Arrange
//            var productId = Guid.NewGuid();
//            _collectionMock.Setup(c => c.DeleteOneAsync(It.IsAny<FilterDefinition<ProductDomain>>(), null, It.IsAny<CancellationToken>())).ReturnsAsync(new DeleteResult.Acknowledged(1));

//            // Act
//            await _repository.DeleteAsync(productId, CancellationToken.None);

//            // Assert
//            _collectionMock.Verify(c => c.DeleteOneAsync(It.IsAny<FilterDefinition<ProductDomain>>(), null, It.IsAny<CancellationToken>()), Times.Once);
//        }

//        [Fact(DisplayName = "Should insert product successfully")]
//        public async Task InsertAsync_Success()
//        {
//            // Arrange
//            var product = new ProductDomain(Guid.NewGuid(), "Product1", 100, 10, "Type1", DateTime.UtcNow, 1);
//            _collectionMock.Setup(c => c.InsertOneAsync(product, null, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
//            _collectionMock.Setup(c => c.Find(It.IsAny<FilterDefinition<ProductDomain>>(), null).FirstOrDefaultAsync(It.IsAny<CancellationToken>())).ReturnsAsync((ProductDomain)null);

//            // Act
//            await _repository.InsertAsync(product, CancellationToken.None);

//            // Assert
//            _collectionMock.Verify(c => c.InsertOneAsync(product, null, It.IsAny<CancellationToken>()), Times.Once);
//        }

//        [Fact(DisplayName = "Should update product successfully")]
//        public async Task UpdateAsync_Success()
//        {
//            // Arrange
//            var product = new ProductDomain(Guid.NewGuid(), "Product1", 100, 10, "Type1", DateTime.UtcNow, 1);
//            _collectionMock.Setup(c => c.UpdateOneAsync(It.IsAny<FilterDefinition<ProductDomain>>(), It.IsAny<UpdateDefinition<ProductDomain>>(), null, It.IsAny<CancellationToken>())).ReturnsAsync(new UpdateResult.Acknowledged(1, 1, null));

//            // Act
//            await _repository.UpdateAsync(product, CancellationToken.None);

//            // Assert
//            _collectionMock.Verify(c => c.UpdateOneAsync(It.IsAny<FilterDefinition<ProductDomain>>(), It.IsAny<UpdateDefinition<ProductDomain>>(), null, It.IsAny<CancellationToken>()), Times.Once);
//        }

//        [Fact(DisplayName = "Should get product by name successfully")]
//        public async Task GetByName_Success()
//        {
//            // Arrange
//            var product = new ProductDomain(Guid.NewGuid(), "Product1", 100, 10, "Type1", DateTime.UtcNow, 1);
//            _collectionMock.Setup(c => c.Find(It.IsAny<FilterDefinition<ProductDomain>>(), null).FirstOrDefaultAsync(It.IsAny<CancellationToken>())).ReturnsAsync(product);

//            // Act
//            var result = await _repository.GetByName("Product1", CancellationToken.None);

//            // Assert
//            Assert.Equal(product, result);
//        }

//        [Fact(DisplayName = "Should get product by id successfully")]
//        public async Task GetById_Success()
//        {
//            // Arrange
//            var product = new ProductDomain(Guid.NewGuid(), "Product1", 100, 10, "Type1", DateTime.UtcNow, 1);
//            _collectionMock.Setup(c => c.Find(It.IsAny<FilterDefinition<ProductDomain>>(), null).FirstOrDefaultAsync(It.IsAny<CancellationToken>())).ReturnsAsync(product);

//            // Act
//            var result = await _repository.GetById(product.Id, CancellationToken.None);

//            // Assert
//            Assert.Equal(product, result);
//        }

//        [Fact(DisplayName = "Should get products by expiration date successfully")]
//        public async Task GetExpiritionByDateAll_Success()
//        {
//            // Arrange
//            var products = new List<ProductDomain>
//            {
//                new ProductDomain(Guid.NewGuid(), "Product1", 100, 10, "Type1", DateTime.UtcNow.AddDays(5), 1),
//                new ProductDomain(Guid.NewGuid(), "Product2", 200, 20, "Type2", DateTime.UtcNow.AddDays(10), 2)
//            };
//            var cursorMock = new Mock<IAsyncCursor<ProductDomain>>();
//            cursorMock.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
//            cursorMock.SetupGet(c => c.Current).Returns(products);
//            _collectionMock.Setup(c => c.FindSync(It.IsAny<FilterDefinition<ProductDomain>>(), It.IsAny<FindOptions<ProductDomain, ProductDomain>>(), It.IsAny<CancellationToken>())).Returns(cursorMock.Object);

//            // Act
//            var result = await _repository.GetExpiritionByDateAll(10, CancellationToken.None);

//            // Assert
//            Assert.Equal(products, result);
//        }
//    }
//}


