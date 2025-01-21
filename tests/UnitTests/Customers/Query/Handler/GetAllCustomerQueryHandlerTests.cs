using Moq;
using Xunit;
using Customers.Command.Handler;
using Customers.Repository.Interface;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Repository.Entities;
using Customers.Command;

namespace UnitTests.Customers.Query.Handler
{

    public class GetAllCustomerQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ICustomerRepository> _repositoryMock;
        private readonly GetAllCustomerQueryHandler _handler;

        public GetAllCustomerQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _repositoryMock = new Mock<ICustomerRepository>();
            _handler = new GetAllCustomerQueryHandler(_mediatorMock.Object, _repositoryMock.Object);
        }

        [Fact(DisplayName = "Handle should return list of CustomerDomain")]
        public async Task Handle_ShouldReturnListOfCustomerDomain()
        {
            // Arrange
            var customers = new List<CustomerDomain>
        {
            new CustomerDomain { Id = 1UL, FullName = "John Doe", User = "johndoe" },
            new CustomerDomain { Id = 2UL, FullName = "Jane Doe", User = "janedoe" }
        };

            _repositoryMock.Setup(r => r.GetAll(It.IsAny<CancellationToken>()))
                .ReturnsAsync(customers);

            var query = new GetAllCustomerQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(customers, result);
        }

        [Fact(DisplayName = "Handle should throw exception when repository throws exception")]
        public async Task Handle_ShouldThrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAll(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Repository exception"));

            var query = new GetAllCustomerQuery();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        }
    }
}
