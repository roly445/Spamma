using MaybeMonad;
using Moq;
using ResultMonad;
using Spamma.Api.Web.Infrastructure.Contracts.Database;
using Spamma.Api.Web.Infrastructure.Contracts.Domain;

namespace Spamma.Shared.Tests
{
    public static class DomainFactory
    {
        public static Mock<IUnitOfWork> CreateSuccessfulUnitOfWork()
        {
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);

            return unitOfWork;
        }

        public static Mock<IUnitOfWork> CreateFailedUnitOfWork()
        {
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail<IPersistenceError>(new UniquePersistenceError()));

            return unitOfWork;
        }

        public static Mock<IRepository<TEntity>> NoData<TEntity>()
            where TEntity : IAggregateRoot
        {
            var repository = new Mock<IRepository<TEntity>>();
            repository.Setup(x => x.FindOne(
                    It.IsAny<Specification<TEntity>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(() => Maybe<TEntity>.Nothing);

            return repository;
        }

        public static Mock<IRepository<TEntity>> Empty<TEntity>(
            Mock<IUnitOfWork> unitOfWork,
            Action<TEntity>? addCallback = null)
            where TEntity : IAggregateRoot
        {
            var repository = new Mock<IRepository<TEntity>>();
            repository.Setup(x => x.UnitOfWork)
                .Returns(unitOfWork.Object);
            if (addCallback != null)
            {
                repository.Setup(x => x.Add(It.IsAny<TEntity>()))
                    .Callback(addCallback);
            }

            return repository;
        }

        public static Mock<IRepository<TEntity>> FindOne<TEntity>(Mock<TEntity> entity)
            where TEntity : class, IAggregateRoot
        {
            var repository = new Mock<IRepository<TEntity>>();
            repository.Setup(x => x.FindOne(
                    It.IsAny<Specification<TEntity>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(() => Maybe.From(entity.Object));

            return repository;
        }

        public static Mock<IRepository<TEntity>> FindOne<TEntity>(
            Mock<TEntity> entity,
            Mock<IUnitOfWork> unitOfWork)
            where TEntity : class, IAggregateRoot
        {
            var repository = new Mock<IRepository<TEntity>>();
            repository.Setup(x => x.FindOne(
                    It.IsAny<Specification<TEntity>>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(() => Maybe.From(entity.Object));
            repository.Setup(x => x.UnitOfWork)
                .Returns(unitOfWork.Object);

            return repository;
        }
    }
}