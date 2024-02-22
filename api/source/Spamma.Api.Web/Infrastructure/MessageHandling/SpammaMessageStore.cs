using System.Buffers;
using MediatR;
using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;
using Spamma.Api.Web.Infrastructure.Constants;
using Spamma.Api.Web.Infrastructure.Contracts.MessageHandling;
using Spamma.Api.Web.Infrastructure.Domain.EmailAggregate.Commands;

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
            var scope = context.ServiceProvider.CreateScope();
            var messageStoreProvider = scope.ServiceProvider.GetRequiredService<IMessageStoreProvider>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await using var stream = new MemoryStream();

            var position = buffer.GetPosition(0);
            while (buffer.TryGet(ref position, out var memory))
            {
                stream.Write(memory.Span);
            }

            stream.Position = 0;

            var message = await MimeKit.MimeMessage.LoadAsync(stream, cancellationToken);

            var messageId = Guid.NewGuid();
            var saveFileResult = await messageStoreProvider.StoreMessageContentAsync(messageId, message, cancellationToken);
            if (!saveFileResult.IsSuccess)
            {
                return SmtpResponse.TransactionFailed;
            }

            var addresses = message.To.Mailboxes.Select(x => new CreateEmailCommand.EmailAddress(x.Address, x.Name, EmailAddressType.To)).ToList();
            addresses.AddRange(message.Cc.Mailboxes.Select(x => new CreateEmailCommand.EmailAddress(x.Address, x.Name, EmailAddressType.Cc)));
            addresses.AddRange(message.Bcc.Mailboxes.Select(x => new CreateEmailCommand.EmailAddress(x.Address, x.Name, EmailAddressType.Bcc)));
            addresses.AddRange(message.From.Mailboxes.Select(x => new CreateEmailCommand.EmailAddress(x.Address, x.Name, EmailAddressType.From)));

            var saveDataResult = await mediator.Send(
                new CreateEmailCommand(
                    messageId,
                    message.Subject,
                    message.Date.DateTime,
                    addresses), cancellationToken);

            if (saveDataResult.Status != CommandResultStatus.Failed)
            {
                return SmtpResponse.Ok;
            }

            await messageStoreProvider.DeleteMessageContentAsync(messageId, cancellationToken);
            return SmtpResponse.TransactionFailed;
        }
    }
}