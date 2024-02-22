using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Spamma.Api.Web.Infrastructure.Contracts.Domain
{
    public abstract class CommandHandler<TCommand, TResult>(IEnumerable<IValidator<TCommand>> validators, ILogger logger)
        : IRequestHandler<TCommand, CommandResult<TResult>>
        where TCommand : ICommand<TResult>
    {
        public async Task<CommandResult<TResult>> Handle(TCommand request, CancellationToken cancellationToken)
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
            return CommandResult<TResult>.Invalid(new ValidationResult(failures));
        }

        protected abstract Task<CommandResult<TResult>> HandleInternal(TCommand request, CancellationToken cancellationToken);
    }
}