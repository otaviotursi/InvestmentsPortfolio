//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Infrastructure.Repository.Entities;
//using MongoDB.Driver;
//using Moq;
//using Portfolio.Repository;

//namespace UnitTests.Portfolio.Repository
//{

//    public class PortfolioRepositoryTests
//    {
//        private readonly Mock<IMongoClient> _mongoClientMock;
//        private readonly Mock<IMongoDatabase> _databaseMock;
//        private readonly Mock<IMongoCollection<PortfolioDomain>> _collectionMock;
//        private readonly PortfolioRepository _repository;

//        public PortfolioRepositoryTests()
//        {
//            _mongoClientMock = new Mock<IMongoClient>();
//            _databaseMock = new Mock<IMongoDatabase>();
//            _collectionMock = new Mock<IMongoCollection<PortfolioDomain>>();

//            _mongoClientMock.Setup(client => client.GetDatabase(It.IsAny<string>(), null))
//                .Returns(_databaseMock.Object);
//            _databaseMock.Setup(db => db.GetCollection<PortfolioDomain>(It.IsAny<string>(), null))
//                .Returns(_collectionMock.Object);

//            _repository = new PortfolioRepository(_mongoClientMock.Object, "testDatabase", "testCollection");
//        }

//        [Fact(DisplayName = "GetAll should return list of PortfolioDomain")]
//        public async Task GetAll_ShouldReturnListOfPortfolioDomain()
//        {
//            // Arrange
//            var portfolioList = new List<PortfolioDomain>
//            {
//                new PortfolioDomain { CustomerId = 1, ItensPortfolio = new List<ItemPortfolio>() },
//                new PortfolioDomain { CustomerId = 2, ItensPortfolio = new List<ItemPortfolio>() }
//            };

//            var cursorMock = new Mock<IAsyncCursor<PortfolioDomain>>();
//            cursorMock.SetupSequence(cursor => cursor.MoveNext(It.IsAny<CancellationToken>()))
//                .Returns(true)
//                .Returns(false);
//            cursorMock.SetupGet(cursor => cursor.Current).Returns(portfolioList);

//            _collectionMock.Setup(collection => collection.FindAsync(Builders<PortfolioDomain>.Filter.Empty,
//                It.IsAny<FindOptions<PortfolioDomain, PortfolioDomain>>(),
//                It.IsAny<CancellationToken>()))
//                .ReturnsAsync(cursorMock.Object);

//            // Act
//            var result = await _repository.GetAll(CancellationToken.None);

//            // Assert
//            Assert.Equal(portfolioList, result);
//        }

//        [Fact(DisplayName = "GetByName should return PortfolioDomain for given customer ID")]
//        public async Task GetByName_ShouldReturnPortfolioDomain_ForGivenCustomerId()
//        {
//            // Arrange
//            var customerId = 1UL;
//            var portfolio = new PortfolioDomain { CustomerId = customerId, ItensPortfolio = new List<ItemPortfolio>() };

//            var cursorMock = new Mock<IAsyncCursor<PortfolioDomain>>();
//            cursorMock.SetupSequence(cursor => cursor.MoveNext(It.IsAny<CancellationToken>()))
//                .Returns(true)
//                .Returns(false);
//            cursorMock.SetupGet(cursor => cursor.Current).Returns(new List<PortfolioDomain> { portfolio });

//            _collectionMock.Setup(collection => collection.FindAsync(
//                It.IsAny<FilterDefinition<PortfolioDomain>>(),
//                It.IsAny<FindOptions<PortfolioDomain, PortfolioDomain>>(),
//                It.IsAny<CancellationToken>()))
//                .ReturnsAsync(cursorMock.Object);

//            // Act
//            var result = await _repository.GetByName(customerId, CancellationToken.None);

//            // Assert
//            Assert.Equal(portfolio, result);
//        }

//        [Fact(DisplayName = "InsertAsync should add new item to portfolio")]
//        public async Task InsertAsync_ShouldAddNewItemToPortfolio()
//        {
//            // Arrange
//            var product = new PortfolioRequest
//            {
//                CustomerId = 1,
//                ProductId = Guid.NewGuid(),
//                ProductName = "Product",
//                AmountNegotiated = 10,
//                ValueNegotiated = 100m
//            };

//            var cursorMock = new Mock<IAsyncCursor<PortfolioDomain>>();
//            cursorMock.SetupSequence(cursor => cursor.MoveNext(It.IsAny<CancellationToken>()))
//                .Returns(false);

//            _collectionMock.Setup(collection => collection.FindAsync(
//                It.IsAny<FilterDefinition<PortfolioDomain>>(),
//                It.IsAny<FindOptions<PortfolioDomain, PortfolioDomain>>(),
//                It.IsAny<CancellationToken>()))
//                .ReturnsAsync(cursorMock.Object);

//            // Act
//            await _repository.InsertAsync(product, CancellationToken.None);

//            // Assert
//            _collectionMock.Verify(collection => collection.UpdateOneAsync(
//                It.IsAny<FilterDefinition<PortfolioDomain>>(),
//                It.IsAny<UpdateDefinition<PortfolioDomain>>(),
//                It.IsAny<UpdateOptions>(),
//                It.IsAny<CancellationToken>()), Times.Once);
//        }

//        [Fact(DisplayName = "RemoveAsync should remove item from portfolio")]
//        public async Task RemoveAsync_ShouldRemoveItemFromPortfolio()
//        {
//            // Arrange
//            var product = new PortfolioRequest
//            {
//                CustomerId = 1,
//                ProductId = Guid.NewGuid(),
//                ProductName = "Product",
//                AmountNegotiated = 10,
//                ValueNegotiated = 100m
//            };

//            var portfolio = new PortfolioDomain
//            {
//                CustomerId = product.CustomerId,
//                ItensPortfolio = new List<ItemPortfolio>
//                {
//                    new ItemPortfolio
//                    {
//                        ProductId = product.ProductId,
//                        ProductName = product.ProductName,
//                        AmountNegotiated = product.AmountNegotiated,
//                        ValueNegotiated = product.ValueNegotiated ?? 0
//                    }
//                }
//            };

//            var cursorMock = new Mock<IAsyncCursor<PortfolioDomain>>();
//            cursorMock.SetupSequence(cursor => cursor.MoveNext(It.IsAny<CancellationToken>()))
//                .Returns(true)
//                .Returns(false);
//            cursorMock.SetupGet(cursor => cursor.Current).Returns(new List<PortfolioDomain> { portfolio });

//            _collectionMock.Setup(collection => collection.FindAsync(
//                It.IsAny<FilterDefinition<PortfolioDomain>>(),
//                It.IsAny<FindOptions<PortfolioDomain, PortfolioDomain>>(),
//                It.IsAny<CancellationToken>()))
//                .ReturnsAsync(cursorMock.Object);

//            // Act
//            await _repository.RemoveAsync(product, CancellationToken.None);

//            // Assert
//            _collectionMock.Verify(collection => collection.ReplaceOneAsync(
//                It.IsAny<FilterDefinition<PortfolioDomain>>(),
//                It.IsAny<PortfolioDomain>(),
//                It.IsAny<ReplaceOptions>(),
//                It.IsAny<CancellationToken>()), Times.Once);
//        }

//        [Fact(DisplayName = "RemoveAsync should delete portfolio when no items remain")]
//        public async Task RemoveAsync_ShouldDeletePortfolio_WhenNoItemsRemain()
//        {
//            // Arrange
//            var product = new PortfolioRequest
//            {
//                CustomerId = 1,
//                ProductId = Guid.NewGuid(),
//                ProductName = "Product",
//                AmountNegotiated = 10,
//                ValueNegotiated = 100m
//            };

//            var portfolio = new PortfolioDomain
//            {
//                CustomerId = product.CustomerId,
//                ItensPortfolio = new List<ItemPortfolio>
//                {
//                    new ItemPortfolio
//                    {
//                        ProductId = product.ProductId,
//                        ProductName = product.ProductName,
//                        AmountNegotiated = product.AmountNegotiated,
//                        ValueNegotiated = product.ValueNegotiated ?? 0
//                    }
//                }
//            };

//            var cursorMock = new Mock<IAsyncCursor<PortfolioDomain>>();
//            cursorMock.SetupSequence(cursor => cursor.MoveNext(It.IsAny<CancellationToken>()))
//                .Returns(true)
//                .Returns(false);
//            cursorMock.SetupGet(cursor => cursor.Current).Returns(new List<PortfolioDomain> { portfolio });

//            _collectionMock.Setup(collection => collection.FindAsync(
//                It.IsAny<FilterDefinition<PortfolioDomain>>(),
//                It.IsAny<FindOptions<PortfolioDomain, PortfolioDomain>>(),
//                It.IsAny<CancellationToken>()))
//                .ReturnsAsync(cursorMock.Object);

//            // Act
//            product.AmountNegotiated = 10; // Remove the exact amount to make the list empty
//            await _repository.RemoveAsync(product, CancellationToken.None);

//            // Assert
//            _collectionMock.Verify(collection => collection.DeleteOneAsync(
//                It.IsAny<FilterDefinition<PortfolioDomain>>(),
//                It.IsAny<CancellationToken>()), Times.Once);
//        }
//    }
//}
