using Moq;
using Xunit;
using Users.Command.Handler;
using Users.Repository.Interface;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Repository.Entities;
using Users.Query.Handler;
using Users.Query;
using Infrastructure.Cache;

namespace UnitTests.Users.Query.Handler
{

    public class GetAllUserQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IUserRepository> _repositoryMock;
        private readonly GetAllUserQueryHandler _handler;
        private readonly Mock<ICacheHelper> _cacheHelper;

        public GetAllUserQueryHandlerTests()
        {
            _cacheHelper = new Mock<ICacheHelper>();
            _repositoryMock = new Mock<IUserRepository>();
            _handler = new GetAllUserQueryHandler(_repositoryMock.Object, _cacheHelper.Object);
        }

        [Fact(DisplayName = "Handle should return list of UserDomain")]
        public async Task Handle_ShouldReturnListOfUserDomain()
        {
            // Arrange
            var customers = new List<UserDomain>
        {
            new UserDomain { Id = 1UL, FullName = "John Doe", User = "johndoe" },
            new UserDomain { Id = 2UL, FullName = "Jane Doe", User = "janedoe" }
        };

            _repositoryMock.Setup(r => r.GetAll(It.IsAny<CancellationToken>()))
                .ReturnsAsync(customers);

            var query = new GetAllUserQuery();

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

            var query = new GetAllUserQuery();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        }
    }
}
