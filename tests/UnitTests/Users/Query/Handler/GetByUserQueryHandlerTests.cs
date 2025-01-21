using Moq;
using Xunit;
using Users.Command.Handler;
using Users.Repository.Interface;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Repository.Entities;
using Users.Query.Handler;
using Users.Query;

namespace UnitTests.Users.Query.Handler
{

    public class GetByUserQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IUserRepository> _repositoryMock;
        private readonly GetByUserQueryHandler _handler;

        public GetByUserQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _repositoryMock = new Mock<IUserRepository>();
            _handler = new GetByUserQueryHandler(_mediatorMock.Object, _repositoryMock.Object);
        }

        [Fact(DisplayName = "Handle should return UserDomain for given query")]
        public async Task Handle_ShouldReturnUserDomain_ForGivenQuery()
        {
            // Arrange
            var customerId = 1UL;
            var customer = new UserDomain { Id = customerId, FullName = "John Doe", User = "johndoe" };

            _repositoryMock.Setup(r => r.GetBy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ulong?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            var query = new GetByUserQuery(customerId, "John Doe", "johndoe");

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

            var query = new GetByUserQuery(customerId, "John Doe", "johndoe");

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        }
    }
}
