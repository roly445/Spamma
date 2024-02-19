using System.Buffers;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using Moq;
using ResultMonad;
using SmtpServer;
using Spamma.Api.Web.Infrastructure.Contracts;
using Spamma.Api.Web.Infrastructure.Contracts.MessageHandling;
using Spamma.Api.Web.Infrastructure.MessageHandling;
using Spamma.Shared.Tests.Mocks;

namespace Spamma.Api.Web.Tests.Infrastructure.MessageHandling
{
    public class SpammaMessageStoreTests
    {
        private readonly ReadOnlySequence<byte> _buffer;
        private readonly Mock<IMessageStoreProvider> _messageStoreProvider = new();
        private readonly MockServiceProvider _mockServiceProvider = new();
        private readonly Mock<ISessionContext> _context = new();

        public SpammaMessageStoreTests()
        {
            var message = new MimeMessage();
            var ms = new MemoryStream();
            message.WriteTo(ms);
            this._buffer = new ReadOnlySequence<byte>(ms.ToArray());
        }

        [Fact]
        public async Task SaveAsync_WhenMessageSaves_ReturnsSmtpResponseOfOk()
        {
            // Arrange
            this._messageStoreProvider.Setup(x => x.StoreMessageContentAsync(
                    It.IsAny<Guid>(), It.IsAny<MimeMessage>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok);
            this._mockServiceProvider.SetServiceCollection(new ServiceCollection()
                .AddScoped<IMessageStoreProvider>(_ => this._messageStoreProvider.Object));
            this._context.Setup(x => x.ServiceProvider)
                .Returns(this._mockServiceProvider.Object);
            var messageStore = new SpammaMessageStore();

            // Act
            var result = await messageStore.SaveAsync(
                this._context.Object, Mock.Of<IMessageTransaction>(), this._buffer, CancellationToken.None);

            // Assert
            await Verify(result);
        }

        [Fact]
        public async Task SaveAsync_WhenMessageFailsToSaves_ReturnsSmtpResponseOfTransactionFailed()
        {
            // Arrange
            this._messageStoreProvider.Setup(x => x.StoreMessageContentAsync(
                    It.IsAny<Guid>(), It.IsAny<MimeMessage>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail);
            this._mockServiceProvider.SetServiceCollection(new ServiceCollection()
                .AddScoped<IMessageStoreProvider>(_ => this._messageStoreProvider.Object));
            this._context.Setup(x => x.ServiceProvider)
                .Returns(this._mockServiceProvider.Object);
            var messageStore = new SpammaMessageStore();

            // Act
            var result = await messageStore.SaveAsync(
                this._context.Object, Mock.Of<IMessageTransaction>(), this._buffer, CancellationToken.None);

            // Assert
            await Verify(result);
        }
    }
}