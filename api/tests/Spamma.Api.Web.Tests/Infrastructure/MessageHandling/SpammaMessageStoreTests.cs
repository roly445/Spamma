using System.Buffers;
using Moq;
using SmtpServer;
using SmtpServer.Protocol;
using Spamma.Api.Web.Infrastructure.MessageHandling;

namespace Spamma.Api.Web.Tests.Infrastructure.MessageHandling
{
    public class SpammaMessageStoreTests
    {
        [Fact]
        public async Task SaveAsync_WhenCalled_ReturnsSmtpResponse()
        {
            // Arrange
            var context = new Mock<ISessionContext>();
            var transaction = new Mock<IMessageTransaction>();
            var buffer = default(ReadOnlySequence<byte>);
            var messageStore = new SpammaMessageStore();

            // Act
            var result = await messageStore.SaveAsync(
                context.Object, transaction.Object, buffer, CancellationToken.None);

            // Assert
            await Verify(result);
        }
    }
}