using FluentValidation;
using Spamma.Api.Web.Infrastructure.Domain.EmailAggregate.Commands;

namespace Spamma.Api.Web.Infrastructure.Domain.EmailAggregate.CommandValidators
{
    public class CreateEmailCommandValidator : AbstractValidator<CreateEmailCommand>;
}