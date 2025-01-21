using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Customers.Command;
using Customers.Command.Handler;
using Customers.Repository;
using Customers.Repository.Interface;
using Infrastructure.Repository.Entities;
using MediatR;
using Moq;

namespace UnitTests.Customers.Command.Handler
{

    public class CreateCustomerCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ICustomerRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CreateCustomerCommandHandler _handler;

        public CreateCustomerCommandHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _repositoryMock = new Mock<ICustomerRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new CreateCustomerCommandHandler(_mediatorMock.Object, _repositoryMock.Object, _mapperMock.Object);
        }

        [Fact(DisplayName = "Handle should create customer successfully")]
        public async Task Handle_ShouldCreateCustomerSuccessfully()
        {
            // Arrange
            var command = new CreateCustomerCommand("John Doe", "johndoe");
            var customer = new CustomerDomain { FullName = "John Doe", User = "johndoe" };

            _mapperMock.Setup(m => m.Map<CustomerDomain>(command)).Returns(customer);
            _repositoryMock.Setup(r => r.InsertAsync(customer, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("Cliente criado com sucesso", result);
            _repositoryMock.Verify(r => r.InsertAsync(customer, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Handle should throw exception when repository throws exception")]
        public async Task Handle_ShouldThrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            var command = new CreateCustomerCommand("John Doe", "johndoe");
            var customer = new CustomerDomain { FullName = "John Doe", User = "johndoe" };

            _mapperMock.Setup(m => m.Map<CustomerDomain>(command)).Returns(customer);
            _repositoryMock.Setup(r => r.InsertAsync(customer, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Repository exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
