using FluentValidation;
using Spamma.Api.Web.Infrastructure.Constants;
using Spamma.Api.Web.Infrastructure.Contracts.Domain;
using Spamma.Api.Web.Infrastructure.Domain.EmailAggregate.Aggregate;
using Spamma.Api.Web.Infrastructure.Domain.EmailAggregate.Commands;

namespace Spamma.Api.Web.Infrastructure.Domain.EmailAggregate.CommandHandlers
{
    public class CreateEmailCommandHandler(IEnumerable<IValidator<CreateEmailCommand>> validators, ILogger<CreateEmailCommandHandler> logger, IRepository<Email> repository) : CommandHandler<CreateEmailCommand>(validators, logger)
    {
        protected override async Task<CommandResult> HandleInternal(CreateEmailCommand request, CancellationToken cancellationToken)
        {
            var email = new Email(
                request.MessageId, request.Subject, request.WhenSent,
                request.EmailAddresses.Select(
                    x => new EmailAddress(x.Address, x.Name, x.EmailAddressType)).ToList());

            repository.Add(email);

            var dbResult = await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (dbResult.IsSuccess)
            {
                return CommandResult.Succeeded();
            }

            logger.LogInformation("Failed saving changes");
            return CommandResult.Failed(new ErrorData(
                ErrorCode.SavingChanges, "Failed to save database"));
        }
    }
}