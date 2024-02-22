using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Spamma.Api.Web.Infrastructure.Constants;
using Spamma.Api.Web.Infrastructure.Domain.EmailAggregate.Aggregate;
using Spamma.Api.Web.Infrastructure.Domain.EmailAggregate.CommandHandlers;
using Spamma.Api.Web.Infrastructure.Domain.EmailAggregate.Commands;
using Spamma.Shared.Tests;

namespace Spamma.Api.Web.Tests.Infrastructure.Domain.EmailAggregate.CommandHandlers
{
    public class CreateEmailCommandHandlerTests
    {
        [Fact]
        public async Task Handle_DataSaves_ExpectNewEmailAndSucceededStatus()
        {
            var command = new CreateEmailCommand(Guid.NewGuid(), "Test", DateTime.UtcNow, new List<CreateEmailCommand.EmailAddress>
            {
                new("address", "name", EmailAddressType.To),
            });

            Email? email = null;

            var repository = DomainFactory.Empty<Email>(DomainFactory.CreateSuccessfulUnitOfWork(), e => email = e);

            var handler = new CreateEmailCommandHandler(
                new List<IValidator<CreateEmailCommand>>(),
                Mock.Of<ILogger<CreateEmailCommandHandler>>(),
                repository.Object);
            var result = await handler.Handle(command, CancellationToken.None);

            await Verify(new
            {
                result.Status,
                email,
            });
        }

        [Fact]
        public async Task Handle_DataFailsToSave_ExpectFailedStatus()
        {
            var command = new CreateEmailCommand(Guid.NewGuid(), "Test", DateTime.UtcNow, new List<CreateEmailCommand.EmailAddress>
            {
                new("address", "name", EmailAddressType.To),
            });

            var repository = DomainFactory.Empty<Email>(DomainFactory.CreateFailedUnitOfWork());

            var handler = new CreateEmailCommandHandler(
                new List<IValidator<CreateEmailCommand>>(),
                Mock.Of<ILogger<CreateEmailCommandHandler>>(),
                repository.Object);
            var result = await handler.Handle(command, CancellationToken.None);

            await Verify(new
            {
                result.Status,
            });
        }
    }
}