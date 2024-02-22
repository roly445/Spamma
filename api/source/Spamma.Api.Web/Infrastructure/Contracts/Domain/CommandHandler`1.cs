using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Spamma.Api.Web.Infrastructure.Contracts.Domain
{
    public abstract class CommandHandler<TCommand>(IEnumerable<IValidator<TCommand>> validators, ILogger logger)
        : IRequestHandler<TCommand, CommandResult>
        where TCommand : ICommand
    {
        public async Task<CommandResult> Handle(TCommand request, CancellationToken cancellationToken)
        {
            var failures = validators
                .Select(v => v.Validate(request))
                .SelectMany(result => result.Errors)
                .Where(error => error != null)
                .ToList();

            if (failures.Count == 0)
            {
                return await this.HandleInternal(request, cancellationToken);
            }

            logger.LogInformation("Command validation failed");
            return CommandResult.Invalid(new ValidationResult(failures));
        }

        protected abstract Task<CommandResult> HandleInternal(TCommand request, CancellationToken cancellationToken);
    }
}