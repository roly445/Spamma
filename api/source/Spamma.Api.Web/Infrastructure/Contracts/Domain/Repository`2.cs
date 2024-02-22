using MaybeMonad;
using Microsoft.EntityFrameworkCore;
using Spamma.Api.Web.Infrastructure.Database;

namespace Spamma.Api.Web.Infrastructure.Contracts.Domain
{
    public abstract class Repository<TAggregateRoot, TEntity>(SpammaDataContext dbContext) : IRepository<TAggregateRoot>
        where TAggregateRoot : IAggregateRoot
        where TEntity : class, TAggregateRoot
    {
        public IUnitOfWork UnitOfWork => dbContext;

        public void Add(TAggregateRoot aggregate)
        {
            if (aggregate is not TEntity entity)
            {
                throw new ArgumentException("Entity not of correct type", nameof(aggregate));
            }

            dbContext.Set<TEntity>().Add(entity);
        }

        public void Update(TAggregateRoot aggregate)
        {
            if (aggregate is not TEntity entity)
            {
                throw new ArgumentException("Entity not of correct type", nameof(aggregate));
            }

            dbContext.Set<TEntity>().Update(entity);
        }

        public void Delete(IAggregateRoot aggregate)
        {
            if (aggregate is not TEntity entity)
            {
                throw new ArgumentException("Entity not of correct type", nameof(aggregate));
            }

            dbContext.Set<TEntity>().Remove(entity);
        }

        public async Task<Maybe<TAggregateRoot>> FindOne(Specification<TAggregateRoot> specification, CancellationToken cancellationToken = default, bool refresh = true)
        {
            var data = await dbContext.Set<TEntity>().SingleOrDefaultAsync(specification.ToExpression(), cancellationToken);
            if (data == null)
            {
                return Maybe<TAggregateRoot>.Nothing;
            }

            if (refresh)
            {
                await this.Refresh(data);
            }

            return Maybe.From(data);
        }

        public async Task<IList<TAggregateRoot>> FindMany(Specification<TAggregateRoot> specification, CancellationToken cancellationToken = default, bool refresh = true)
        {
            var data = await dbContext.Set<TEntity>().Where(specification.ToExpression())
                .ToListAsync(cancellationToken);

            if (refresh)
            {
                foreach (TAggregateRoot entity in data)
                {
                    await this.Refresh(entity);
                }
            }

            return data;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            dbContext.Dispose();
        }

        private async Task Refresh(TAggregateRoot? aggregate)
        {
            if (aggregate != null)
            {
                await dbContext.Entry(aggregate).ReloadAsync();
            }
        }
    }
}