using ResultMonad;

namespace Spamma.Api.Web.Infrastructure.Contracts.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        Task<ResultWithError<IPersistenceError>> SaveEntitiesAsync(CancellationToken cancellationToken = default);
    }
}