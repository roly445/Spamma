using MimeKit;
using ResultMonad;

namespace Spamma.Api.Web.Infrastructure.Contracts.MessageHandling
{
    public interface IMessageStoreProvider
    {
        ValueTask<Result> StoreMessageContentAsync(Guid messageId, MimeMessage messageContent, CancellationToken cancellationToken = default);
    }
}