using Spamma.Api.Web.Infrastructure.Constants;
using Spamma.Api.Web.Infrastructure.Contracts.Domain;

namespace Spamma.Api.Web.Infrastructure.Domain.EmailAggregate.Commands
{
    public record CreateEmailCommand(
        Guid MessageId,
        string Subject,
        DateTime WhenSent,
        IReadOnlyList<CreateEmailCommand.EmailAddress> EmailAddresses) : ICommand
    {
        public record EmailAddress(
            string Address,
            string Name,
            EmailAddressType EmailAddressType);
    }
}