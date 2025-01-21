using AutoMapper;
using Moq;
using Xunit;
using Users.Command.Handler;
using Users.Repository.Interface;
using Infrastructure.Repository.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Users.Command;

namespace UnitTests.Users.Command.Handler
{

    public class UpdateUserCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IUserRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UpdateUserCommandHandler _handler;

        public UpdateUserCommandHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _repositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new UpdateUserCommandHandler(_mediatorMock.Object, _repositoryMock.Object, _mapperMock.Object);
        }

        [Fact(DisplayName = "Handle should update customer successfully")]
        public async Task Handle_ShouldUpdateUserSuccessfully()
        {
            // Arrange
            var command = new UpdateUserCommand(1UL, "John Doe", "johndoe");
            var customer = new UserDomain { Id = 1UL, FullName = "John Doe", User = "johndoe" };

            _mapperMock.Setup(m => m.Map<UserDomain>(command)).Returns(customer);
            _repositoryMock.Setup(r => r.UpdateAsync(customer, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("Usuário 1 alterado com sucesso", result);
            _repositoryMock.Verify(r => r.UpdateAsync(customer, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Handle should throw exception when repository throws exception")]
        public async Task Handle_ShouldThrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            var command = new UpdateUserCommand(1UL, "John Doe", "johndoe");
            var customer = new UserDomain { Id = 1UL, FullName = "John Doe", User = "johndoe" };

            _mapperMock.Setup(m => m.Map<UserDomain>(command)).Returns(customer);
            _repositoryMock.Setup(r => r.UpdateAsync(customer, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Repository exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
