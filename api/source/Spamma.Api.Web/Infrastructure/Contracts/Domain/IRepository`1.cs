using MaybeMonad;

namespace Spamma.Api.Web.Infrastructure.Contracts.Domain
{
    public interface IRepository<TAggregateRoot> : IDisposable
        where TAggregateRoot : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }

        void Add(TAggregateRoot aggregate);

        void Update(TAggregateRoot aggregate);

        void Delete(IAggregateRoot aggregate);

        Task<Maybe<TAggregateRoot>> FindOne(Specification<TAggregateRoot> specification, CancellationToken cancellationToken = default, bool refresh = true);

        Task<IList<TAggregateRoot>> FindMany(Specification<TAggregateRoot> specification, CancellationToken cancellationToken = default, bool refresh = true);
    }
}