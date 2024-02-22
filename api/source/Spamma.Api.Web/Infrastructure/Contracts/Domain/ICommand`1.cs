using MediatR;

namespace Spamma.Api.Web.Infrastructure.Contracts.Domain
{
    public interface ICommand<T> : IRequest<CommandResult<T>>;
}