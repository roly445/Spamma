using System.Buffers;
using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;
using Spamma.Api.Web.Infrastructure.Contracts;
using Spamma.Api.Web.Infrastructure.Contracts.MessageHandling;

namespace Spamma.Api.Web.Infrastructure.MessageHandling
{
    public class SpammaMessageStore : MessageStore
    {
        public override async Task<SmtpResponse> SaveAsync(
            ISessionContext context,
            IMessageTransaction transaction,
            ReadOnlySequence<byte> buffer,
            CancellationToken cancellationToken)
        {
            await using var stream = new MemoryStream();

            var position = buffer.GetPosition(0);
            while (buffer.TryGet(ref position, out var memory))
            {
                stream.Write(memory.Span);
            }

            stream.Position = 0;

            var message = await MimeKit.MimeMessage.LoadAsync(stream, cancellationToken);

            var messageStoreProvider = context.ServiceProvider.GetRequiredService<IMessageStoreProvider>();
            var result = await messageStoreProvider.StoreMessageContentAsync(Guid.NewGuid(), message, cancellationToken);

            return result.IsSuccess
                ? SmtpResponse.Ok
                : SmtpResponse.TransactionFailed;
        }
    }
}