using MaybeMonad;
using Spamma.Api.Web.Infrastructure.Contracts.Domain;
using Spamma.Api.Web.Infrastructure.Database;

namespace Spamma.Api.Web.Tests.Infrastructure.Domain
{
    [Collection("Sqlite collection")]
    public abstract class RepositoryTests<TEntity, TRepository>()
        where TEntity : class, IAggregateRoot
        where TRepository : Repository<TEntity, TEntity>
    {
        private readonly SpammaDataContext _dataContext = new();

        protected abstract Specification<TEntity> FindValidSpec { get; }

        protected abstract Specification<TEntity> FindMultipleValidSpec { get; }

        protected abstract Specification<TEntity> FindInvalidSpec { get; }

        protected abstract Specification<TEntity> FindMultipleInvalidSpec { get; }

        [Fact]
        public async Task Add_WhenEntityIsCorrect_ExpectsEntityToBeAdded()
        {
            // Arrange
            var repositoryMaybe = this.CreateRepository();
            if (repositoryMaybe.HasNoValue)
            {
                throw new InvalidOperationException("Repository not found");
            }

            var repository = repositoryMaybe.Value;
            var data = this.GenerateSeedEntity();

            // Act
            repository.Add(data);

            // Assert
            await Verify(this._dataContext.ChangeTracker.Entries().Select(x => new
            {
                x.Entity,
                x.State,
            }));
            this._dataContext.ChangeTracker.Clear();
        }

        [Fact]
        public async Task Add_WhenEntityIsNotCorrect_ExpectsException()
        {
            // Arrange
            TEntity? entity = null;
            var repositoryMaybe = this.CreateRepository();
            if (repositoryMaybe.HasNoValue)
            {
                throw new InvalidOperationException("Repository not found");
            }

            var repository = repositoryMaybe.Value;

            // Act
            var exception = Assert.Throws<ArgumentException>(() => repository.Add(entity!));

            // Assert
            await Verify(new
            {
                Type = exception.GetType(),
                exception.Message,
                exception.ParamName,
            });
        }

        [Fact]
        public async Task Update_WhenEntityIsCorrect_ExpectsEntityToBeUpdate()
        {
            // Arrange
            var repositoryMaybe = this.CreateRepository();
            if (repositoryMaybe.HasNoValue)
            {
                throw new InvalidOperationException("Repository not found");
            }

            var repository = repositoryMaybe.Value;
            var data = this.GenerateSeedEntity();

            // Act
            repository.Update(data);

            // Assert
            await Verify(this._dataContext.ChangeTracker.Entries().Select(x => new
            {
                x.Entity,
                x.State,
            }));
            this._dataContext.ChangeTracker.Clear();
        }

        [Fact]
        public async Task Update_WhenEntityIsNotCorrect_ExpectsException()
        {
            // Arrange
            TEntity? entity = null;
            var repositoryMaybe = this.CreateRepository();
            if (repositoryMaybe.HasNoValue)
            {
                throw new InvalidOperationException("Repository not found");
            }

            var repository = repositoryMaybe.Value;

            // Act
            var exception = Assert.Throws<ArgumentException>(() => repository.Update(entity!));

            // Assert
            await Verify(new
            {
                Type = exception.GetType(),
                exception.Message,
                exception.ParamName,
            });
        }

        [Fact]
        public async Task Delete_WhenEntityIsCorrect_ExpectsEntityToBeDeleted()
        {
            // Arrange
            var repositoryMaybe = this.CreateRepository();
            if (repositoryMaybe.HasNoValue)
            {
                throw new InvalidOperationException("Repository not found");
            }

            var repository = repositoryMaybe.Value;
            var data = this.GenerateSeedEntity();

            // Act
            repository.Delete(data);

            // Assert
            await Verify(this._dataContext.ChangeTracker.Entries().Select(x => new
            {
                x.Entity,
                x.State,
            }));
            this._dataContext.ChangeTracker.Clear();
        }

        [Fact]
        public async Task Delete_WhenEntityIsNotCorrect_ExpectsException()
        {
            // Arrange
            TEntity? entity = null;
            var repositoryMaybe = this.CreateRepository();
            if (repositoryMaybe.HasNoValue)
            {
                throw new InvalidOperationException("Repository not found");
            }

            var repository = repositoryMaybe.Value;

            // Act
            var exception = Assert.Throws<ArgumentException>(() => repository.Delete(entity!));

            // Assert
            await Verify(new
            {
                Type = exception.GetType(),
                exception.Message,
                exception.ParamName,
            });
        }

        [Fact]
        public async Task FindOne_WhenDataMatchesSpecification_ExpectsEntityToBeFound()
        {
            // Arrange
            var repositoryMaybe = this.CreateRepository();
            if (repositoryMaybe.HasNoValue)
            {
                throw new InvalidOperationException("Repository not found");
            }

            var repository = repositoryMaybe.Value;

            var data = this.GenerateSeedEntities();
            this._dataContext.AddRange(data);
            await this._dataContext.SaveChangesAsync();

            // Act
            var result = await repository.FindOne(this.FindValidSpec);

            // Assert
            await Verify(new
            {
                result.HasValue,
                result.Value,
            });
            this._dataContext.Set<TEntity>().RemoveRange(data);
            await this._dataContext.SaveChangesAsync();
        }

        [Fact]
        public async Task FindOne_WhenDataDoesNotMatchSpecification_ExpectsNoEntityFound()
        {
            // Arrange
            var repositoryMaybe = this.CreateRepository();
            if (repositoryMaybe.HasNoValue)
            {
                throw new InvalidOperationException("Repository not found");
            }

            var repository = repositoryMaybe.Value;

            var data = this.GenerateSeedEntities();
            this._dataContext.AddRange(data);
            await this._dataContext.SaveChangesAsync();

            // Act
            var result = await repository.FindOne(this.FindInvalidSpec);

            // Assert
            await Verify(result.HasNoValue);
            this._dataContext.Set<TEntity>().RemoveRange(data);
            await this._dataContext.SaveChangesAsync();
        }

        [Fact]
        public async Task FindMany_WhenDataMatchesSpecification_ExpectsEntityToBeFound()
        {
            // Arrange
            var repositoryMaybe = this.CreateRepository();
            if (repositoryMaybe.HasNoValue)
            {
                throw new InvalidOperationException("Repository not found");
            }

            var repository = repositoryMaybe.Value;

            var data = this.GenerateSeedEntities();
            this._dataContext.AddRange(data);
            await this._dataContext.SaveChangesAsync();

            // Act
            var result = await repository.FindMany(this.FindMultipleValidSpec);

            // Assert
            await Verify(result);
            this._dataContext.Set<TEntity>().RemoveRange(data);
            await this._dataContext.SaveChangesAsync();
        }

        [Fact]
        public async Task FindMany_WhenDataDoesNotMatchSpecification_ExpectsNoEntityFound()
        {
            // Arrange
            var repositoryMaybe = this.CreateRepository();
            if (repositoryMaybe.HasNoValue)
            {
                throw new InvalidOperationException("Repository not found");
            }

            var repository = repositoryMaybe.Value;

            var data = this.GenerateSeedEntities();
            this._dataContext.AddRange(data);
            await this._dataContext.SaveChangesAsync();

            // Act
            var result = await repository.FindMany(this.FindMultipleInvalidSpec);

            // Assert
            await Verify(result);
            this._dataContext.Set<TEntity>().RemoveRange(data);
            await this._dataContext.SaveChangesAsync();
        }

        protected abstract TEntity GenerateSeedEntity();

        protected abstract IReadOnlyList<TEntity> GenerateSeedEntities();

        private Maybe<TRepository> CreateRepository()
        {
            var type = typeof(TRepository);
            var constructor = type.GetConstructor(new[] { typeof(SpammaDataContext) });
            return constructor == null ? Maybe<TRepository>.Nothing : Maybe.From((TRepository)constructor.Invoke([
                this._dataContext
            ]));
        }
    }
}