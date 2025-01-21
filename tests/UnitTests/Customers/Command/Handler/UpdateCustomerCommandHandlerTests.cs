using AutoMapper;
using Moq;
using Xunit;
using Customers.Command.Handler;
using Customers.Repository.Interface;
using Infrastructure.Repository.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Customers.Command;

namespace UnitTests.Customers.Command.Handler
{

    public class UpdateCustomerCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ICustomerRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UpdateCustomerCommandHandler _handler;

        public UpdateCustomerCommandHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _repositoryMock = new Mock<ICustomerRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new UpdateCustomerCommandHandler(_mediatorMock.Object, _repositoryMock.Object, _mapperMock.Object);
        }

        [Fact(DisplayName = "Handle should update customer successfully")]
        public async Task Handle_ShouldUpdateCustomerSuccessfully()
        {
            // Arrange
            var command = new UpdateCustomerCommand(1UL, "John Doe", "johndoe");
            var customer = new CustomerDomain { Id = 1UL, FullName = "John Doe", User = "johndoe" };

            _mapperMock.Setup(m => m.Map<CustomerDomain>(command)).Returns(customer);
            _repositoryMock.Setup(r => r.UpdateAsync(customer, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("Cliente 1 alterado com sucesso", result);
            _repositoryMock.Verify(r => r.UpdateAsync(customer, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Handle should throw exception when repository throws exception")]
        public async Task Handle_ShouldThrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            var command = new UpdateCustomerCommand(1UL, "John Doe", "johndoe");
            var customer = new CustomerDomain { Id = 1UL, FullName = "John Doe", User = "johndoe" };

            _mapperMock.Setup(m => m.Map<CustomerDomain>(command)).Returns(customer);
            _repositoryMock.Setup(r => r.UpdateAsync(customer, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Repository exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
