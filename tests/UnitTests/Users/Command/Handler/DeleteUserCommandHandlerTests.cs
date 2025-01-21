using Moq;
using Xunit;
using Customers.Command.Handler;
using Customers.Repository.Interface;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Customers.Command;

namespace UnitTests.Users.Command.Handler
{

    public class DeleteUserCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ICustomerRepository> _repositoryMock;
        private readonly DeleteCustomerCommandHandler _handler;

        public DeleteUserCommandHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _repositoryMock = new Mock<ICustomerRepository>();
            _handler = new DeleteCustomerCommandHandler(_mediatorMock.Object, _repositoryMock.Object);
        }

        [Fact(DisplayName = "Handle should delete customer successfully")]
        public async Task Handle_ShouldDeleteCustomerSuccessfully()
        {
            // Arrange
            var command = new DeleteCustomerCommand(1UL);

            _repositoryMock.Setup(r => r.DeleteAsync(command.Id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("Cliente excluido com sucesso", result);
            _repositoryMock.Verify(r => r.DeleteAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Handle should throw exception when repository throws exception")]
        public async Task Handle_ShouldThrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            var command = new DeleteCustomerCommand(1UL);

            _repositoryMock.Setup(r => r.DeleteAsync(command.Id, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Repository exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
