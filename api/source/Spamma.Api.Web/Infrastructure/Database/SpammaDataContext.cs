using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using ResultMonad;
using Spamma.Api.Web.Infrastructure.Contracts.Database;
using Spamma.Api.Web.Infrastructure.Contracts.Domain;
using Spamma.Api.Web.Infrastructure.Database.TypeConfigurations;

namespace Spamma.Api.Web.Infrastructure.Database
{
    public class SpammaDataContext : DbContext, IUnitOfWork
    {
        public async Task<ResultWithError<IPersistenceError>> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await this.SaveChangesAsync(cancellationToken);
            }
            catch (UniqueConstraintException)
            {
                return ResultWithError.Fail<IPersistenceError>(new UniquePersistenceError());
            }
            catch (ReferenceConstraintException)
            {
                return ResultWithError.Fail<IPersistenceError>(new InUsePersistenceError());
            }

            return ResultWithError.Ok<IPersistenceError>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EmailConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=spamma.db");
            base.OnConfiguring(optionsBuilder);
        }
    }
}