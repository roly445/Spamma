using System.Buffers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using Moq;
using ResultMonad;
using SmtpServer;
using Spamma.Api.Web.Infrastructure.Constants;
using Spamma.Api.Web.Infrastructure.Contracts.Domain;
using Spamma.Api.Web.Infrastructure.Contracts.MessageHandling;
using Spamma.Api.Web.Infrastructure.Domain.EmailAggregate.Commands;
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
        private readonly Mock<IMediator> _mediator = new();

        public SpammaMessageStoreTests()
        {
            var message = new MimeMessage();
            var ms = new MemoryStream();
            message.WriteTo(ms);
            this._buffer = new ReadOnlySequence<byte>(ms.ToArray());
        }

        [Fact]
        public async Task SaveAsync_WhenMessageSavesCompletely_ReturnsSmtpResponseOfOk()
        {
            // Arrange
            this._messageStoreProvider.Setup(x => x.StoreMessageContentAsync(
                    It.IsAny<Guid>(), It.IsAny<MimeMessage>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok);
            this._messageStoreProvider.Setup(x =>
                x.DeleteMessageContentAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()));

            this._mediator.Setup(x => x.Send(It.IsAny<CreateEmailCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CommandResult.Succeeded());

            this._mockServiceProvider.SetServiceCollection(new ServiceCollection()
                .AddScoped<IMessageStoreProvider>(_ => this._messageStoreProvider.Object)
                .AddScoped<IMediator>(_ => this._mediator.Object));
            this._context.Setup(x => x.ServiceProvider)
                .Returns(this._mockServiceProvider.Object);
            var messageStore = new SpammaMessageStore();

            // Act
            var result = await messageStore.SaveAsync(
                this._context.Object, Mock.Of<IMessageTransaction>(), this._buffer, CancellationToken.None);

            // Assert
            this._messageStoreProvider.Verify(x => x.DeleteMessageContentAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
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
                .AddScoped<IMessageStoreProvider>(_ => this._messageStoreProvider.Object)
                .AddScoped<IMediator>(_ => this._mediator.Object));
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
        public async Task SaveAsync_WhenDataDoesNotSaves_ReturnsSmtpResponseOfTransactionFailed()
        {
            // Arrange
            this._messageStoreProvider.Setup(x => x.StoreMessageContentAsync(
                    It.IsAny<Guid>(), It.IsAny<MimeMessage>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok);
            this._messageStoreProvider.Setup(x =>
                x.DeleteMessageContentAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()));

            this._mediator.Setup(x => x.Send(It.IsAny<CreateEmailCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CommandResult.Failed(new ErrorData(ErrorCode.SavingChanges)));

            this._mockServiceProvider.SetServiceCollection(new ServiceCollection()
                .AddScoped<IMessageStoreProvider>(_ => this._messageStoreProvider.Object)
                .AddScoped<IMediator>(_ => this._mediator.Object));
            this._context.Setup(x => x.ServiceProvider)
                .Returns(this._mockServiceProvider.Object);
            var messageStore = new SpammaMessageStore();

            // Act
            var result = await messageStore.SaveAsync(
                this._context.Object, Mock.Of<IMessageTransaction>(), this._buffer, CancellationToken.None);

            // Assert
            this._messageStoreProvider.Verify(x => x.DeleteMessageContentAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            await Verify(result);
        }
    }
}