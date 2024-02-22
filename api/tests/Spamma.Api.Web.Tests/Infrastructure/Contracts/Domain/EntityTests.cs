using MediatR;
using Spamma.Api.Web.Infrastructure.Contracts.Domain;

namespace Spamma.Api.Web.Tests.Infrastructure.Contracts.Domain
{
    public class EntityTests
    {
        [Fact]
        public async Task Id_WhenConstructed_ExpectDefault()
        {
            var entity = new StubEntity(Guid.NewGuid());

            await Verify(entity);
        }

        [Fact]
        public async Task AddDomainEvent_WhenCalled_ExpectDomainEventInList()
        {
            var entity = new StubEntity(Guid.NewGuid());

            entity.AddDomainEvent(new StubNotification());

            await Verify(entity);
        }

        [Fact]
        public async Task RemoveDomainEvent_WhenCalled_ExpectDomainEventNotInList()
        {
            var entity = new StubEntity(Guid.NewGuid());
            var notification = new StubNotification();
            entity.AddDomainEvent(notification);
            entity.AddDomainEvent(new StubNotification());

            entity.RemoveDomainEvent(notification);

            await Verify(entity);
        }

        [Fact]
        public async Task ClearDomainEvents_WhenCalled_ExpectNoDomainEvents()
        {
            var entity = new StubEntity(Guid.NewGuid());
            entity.AddDomainEvent(new StubNotification());
            entity.AddDomainEvent(new StubNotification());

            entity.ClearDomainEvents();

            await Verify(entity);
        }

        [Fact]
        public async Task Equals_WhenOtherIsNull_ExpectFalse()
        {
            var entity = new StubEntity(Guid.NewGuid());

            var result = entity.Equals(null);

            await Verify(result);
        }

        [Fact]
        public async Task Equals_WhenOtherIsSameReference_ExpectTrue()
        {
            var entity = new StubEntity(Guid.NewGuid());

            var result = entity.Equals(entity);

            await Verify(result);
        }

        [Fact]
        public async Task Equals_WhenOtherIsNotEntity_ExpectFalse()
        {
            var entity = new StubEntity(Guid.NewGuid());

            var result = entity.Equals(new object());

            await Verify(result);
        }

        [Fact]
        public async Task ToString_WhenCalled_ExpectTypeNameAndId()
        {
            var entity = new StubEntity(Guid.Parse("3df7f329-a63d-44e5-bbff-484944c22803"));

            var result = entity.ToString();

            await Verify(result);
        }

        private class StubEntity(Guid id)
            : Entity(id);

        private record StubNotification
            : INotification
        {
            public Guid Id { get; } = Guid.NewGuid();
        }
    }
}