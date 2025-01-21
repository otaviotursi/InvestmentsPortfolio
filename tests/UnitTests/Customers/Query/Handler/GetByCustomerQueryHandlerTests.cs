using Moq;
using Xunit;
using Customers.Command.Handler;
using Customers.Repository.Interface;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Repository.Entities;
using Customers.Command;

namespace UnitTests.Customers.Query.Handler
{

    public class GetByCustomerQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ICustomerRepository> _repositoryMock;
        private readonly GetByCustomerQueryHandler _handler;

        public GetByCustomerQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _repositoryMock = new Mock<ICustomerRepository>();
            _handler = new GetByCustomerQueryHandler(_mediatorMock.Object, _repositoryMock.Object);
        }

        [Fact(DisplayName = "Handle should return CustomerDomain for given query")]
        public async Task Handle_ShouldReturnCustomerDomain_ForGivenQuery()
        {
            // Arrange
            var customerId = 1UL;
            var customer = new CustomerDomain { Id = customerId, FullName = "John Doe", User = "johndoe" };

            _repositoryMock.Setup(r => r.GetBy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ulong?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            var query = new GetByCustomerQuery(customerId, "John Doe", "johndoe");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(customer, result);
        }

        [Fact(DisplayName = "Handle should throw exception when repository throws exception")]
        public async Task Handle_ShouldThrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            var customerId = 1UL;

            _repositoryMock.Setup(r => r.GetBy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ulong?>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Repository exception"));

            var query = new GetByCustomerQuery(customerId, "John Doe", "johndoe");

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        }
    }
}
