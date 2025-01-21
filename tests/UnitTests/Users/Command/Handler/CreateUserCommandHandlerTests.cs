using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Repository.Entities;
using MediatR;
using Moq;
using Users.Command;
using Users.Command.Handler;
using Users.Repository.Interface;

namespace UnitTests.Users.Command.Handler
{

    public class CreateUserCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IUserRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CreateUserCommandHandler _handler;

        public CreateUserCommandHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _repositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new CreateUserCommandHandler(_mediatorMock.Object, _repositoryMock.Object, _mapperMock.Object);
        }

        [Fact(DisplayName = "Handle should create customer successfully")]
        public async Task Handle_ShouldCreateUserSuccessfully()
        {
            // Arrange
            var command = new CreateUserCommand("John Doe", "johndoe");
            var customer = new UserDomain { FullName = "John Doe", User = "johndoe" };

            _mapperMock.Setup(m => m.Map<UserDomain>(command)).Returns(customer);
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
            var command = new CreateUserCommand("John Doe", "johndoe");
            var customer = new UserDomain { FullName = "John Doe", User = "johndoe" };

            _mapperMock.Setup(m => m.Map<UserDomain>(command)).Returns(customer);
            _repositoryMock.Setup(r => r.InsertAsync(customer, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Repository exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
