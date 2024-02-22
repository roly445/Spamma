using MediatR;

namespace Spamma.Api.Web.Infrastructure.Contracts.Domain
{
    public interface ICommand : IRequest<CommandResult>;
}