using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;
using Moq;
using Spamma.Api.Web.Infrastructure.Contracts.SutWrappers;
using Spamma.Api.Web.Infrastructure.MessageHandling;

namespace Spamma.Api.Web.Tests.Infrastructure.MessageHandling
{
    public class LocalMessageStoreProviderTests
    {
        private readonly MimeMessage _message = new();
        private readonly Mock<IHostEnvironment> _hostEnvironment = new();
        private readonly Mock<IDirectoryWrapper> _directoryWrapper = new();
        private readonly Mock<IFileWrapper> _fileWrapper = new();
        private readonly Mock<ILogger<LocalMessageStoreProvider>> _logger = new();
        private readonly string _testPath = Path.Combine(Path.GetTempPath(), $"spamma");

        [Fact]
        public async Task StoreMessageContentAsync_WhenMessageSaves_ExpectOkResult()
        {
            // Arrange
            var testPath = Path.Combine(this._testPath, $"test{Guid.NewGuid()}");
            var messagePath = Path.Combine(testPath, "messages");
            Directory.CreateDirectory(messagePath);
            this._hostEnvironment.Setup(e => e.ContentRootPath)
                .Returns(testPath);

            this._directoryWrapper.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);

            var messageStoreProvider = new LocalMessageStoreProvider(
                this._hostEnvironment.Object, this._logger.Object, this._directoryWrapper.Object, this._fileWrapper.Object);

            // Act
            var result = await messageStoreProvider.StoreMessageContentAsync(
                Guid.NewGuid(), this._message, CancellationToken.None);

            // Assert
            await Verify(new
            {
                IsSuccessful = result.IsSuccess,
                HasFiles = Directory.GetFiles(messagePath).Length > 0,
            });

            // Cleanup
            Directory.Delete(testPath, true);
        }

        [Fact]
        public async Task StoreMessageContentAsync_WhenDirectoryDoesntExist_ExpectOneToBeCreate()
        {
            // Arrange
            var testPath = Path.Combine(this._testPath, $"test{Guid.NewGuid()}");
            var messagePath = Path.Combine(testPath, "messages");
            this._hostEnvironment.Setup(e => e.ContentRootPath)
                .Returns(testPath);

            this._directoryWrapper.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);
            this._directoryWrapper.Setup(x => x.CreateDirectory(It.IsAny<string>()))
                .Callback((string p) => Directory.CreateDirectory(p));

            var messageStoreProvider = new LocalMessageStoreProvider(
                this._hostEnvironment.Object, this._logger.Object, this._directoryWrapper.Object, this._fileWrapper.Object);

            // Act
            var result = await messageStoreProvider.StoreMessageContentAsync(
                Guid.NewGuid(), this._message, CancellationToken.None);

            // Assert
            await Verify(new
            {
                IsSuccessful = result.IsSuccess,
                HasFiles = Directory.GetFiles(messagePath).Length > 0,
            });

            // Cleanup
            Directory.Delete(testPath, true);
        }

        [Fact]
        public async Task StoreMessageContentAsync_WhenUnauthorizedAccessExceptionIsThrown_ExpectFailure()
        {
            // Arrange
            this._hostEnvironment.Setup(e => e.ContentRootPath)
                .Returns(Path.GetTempPath());

            this._directoryWrapper.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);
            this._directoryWrapper.Setup(x => x.CreateDirectory(It.IsAny<string>()))
                .Throws<UnauthorizedAccessException>();

            var messageStoreProvider = new LocalMessageStoreProvider(
                this._hostEnvironment.Object, this._logger.Object, this._directoryWrapper.Object, this._fileWrapper.Object);

            // Act
            var result = await messageStoreProvider.StoreMessageContentAsync(
                Guid.NewGuid(), this._message, CancellationToken.None);

            // Assert
            await Verify(result.IsFailure);
        }

        [Fact]
        public async Task StoreMessageContentAsync_WhenPathTooLongExceptionIsThrown_ExpectFailure()
        {
            // Arrange
            this._hostEnvironment.Setup(e => e.ContentRootPath)
                .Returns(Path.GetTempPath());

            this._directoryWrapper.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);
            this._directoryWrapper.Setup(x => x.CreateDirectory(It.IsAny<string>()))
                .Throws<PathTooLongException>();

            var messageStoreProvider = new LocalMessageStoreProvider(
                this._hostEnvironment.Object, this._logger.Object, this._directoryWrapper.Object, this._fileWrapper.Object);

            // Act
            var result = await messageStoreProvider.StoreMessageContentAsync(
                Guid.NewGuid(), this._message, CancellationToken.None);

            // Assert
            await Verify(result.IsFailure);
        }

        [Fact]
        public async Task StoreMessageContentAsync_WhenDirectoryNotFoundExceptionIsThrown_ExpectFailure()
        {
            // Arrange
            this._hostEnvironment.Setup(e => e.ContentRootPath)
                .Returns(Path.GetTempPath());

            this._directoryWrapper.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);
            this._directoryWrapper.Setup(x => x.CreateDirectory(It.IsAny<string>()))
                .Throws<DirectoryNotFoundException>();

            var messageStoreProvider = new LocalMessageStoreProvider(
                this._hostEnvironment.Object, this._logger.Object, this._directoryWrapper.Object, this._fileWrapper.Object);

            // Act
            var result = await messageStoreProvider.StoreMessageContentAsync(
                Guid.NewGuid(), this._message, CancellationToken.None);

            // Assert
            await Verify(result.IsFailure);
        }

        [Fact]
        public async Task DeleteMessageContentAsync_WhenDirectoryDoesntExist_ExpectFailedResult()
        {
            // Arrange
            var testPath = Path.Combine(this._testPath, $"test{Guid.NewGuid()}");
            this._hostEnvironment.Setup(e => e.ContentRootPath)
                .Returns(testPath);

            this._directoryWrapper.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);

            var messageStoreProvider = new LocalMessageStoreProvider(
                this._hostEnvironment.Object, this._logger.Object, this._directoryWrapper.Object, this._fileWrapper.Object);

            // Act
            var result = await messageStoreProvider.DeleteMessageContentAsync(
                Guid.NewGuid(), CancellationToken.None);

            // Assert
            await Verify(new
            {
                IsSuccessful = result.IsSuccess,
            });
        }

        [Fact]
        public async Task DeleteMessageContentAsync_WhenDirectoryExists_ExpectFileDeletedAndOkResult()
        {
            // Arrange
            var testPath = Path.Combine(this._testPath, $"test{Guid.NewGuid()}");
            this._hostEnvironment.Setup(e => e.ContentRootPath)
                .Returns(testPath);

            this._directoryWrapper.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
            this._fileWrapper.Setup(x => x.Delete(It.IsAny<string>()));

            var messageStoreProvider = new LocalMessageStoreProvider(
                this._hostEnvironment.Object, this._logger.Object, this._directoryWrapper.Object, this._fileWrapper.Object);

            // Act
            var result = await messageStoreProvider.DeleteMessageContentAsync(
                Guid.NewGuid(), CancellationToken.None);

            // Assert
            this._fileWrapper.Verify(x => x.Delete(It.IsAny<string>()), Times.Once);
            await Verify(new
            {
                IsSuccessful = result.IsSuccess,
            });
        }
    }
}