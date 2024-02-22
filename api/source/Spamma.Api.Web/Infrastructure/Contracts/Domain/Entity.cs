using MediatR;

namespace Spamma.Api.Web.Infrastructure.Contracts.Domain
{
    public abstract class Entity
    {
        private readonly List<INotification> _domainEvents = new();

        protected Entity(Guid id)
        {
            this.Id = id;
        }

        protected Entity()
        {
        }

        public Guid Id { get; protected set; }

        public IReadOnlyCollection<INotification> DomainEvents => this._domainEvents.AsReadOnly();

        public void AddDomainEvent(INotification domainEvent)
        {
            this._domainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(INotification domainEvent)
        {
            this._domainEvents?.Remove(domainEvent);
        }

        public void ClearDomainEvents()
        {
            this._domainEvents?.Clear();
        }

        public override bool Equals(object? obj)
        {
            var compareTo = obj as Entity;

            if (ReferenceEquals(this, compareTo))
            {
                return true;
            }

            return !ReferenceEquals(null, compareTo) && this.Id.Equals(compareTo.Id);
        }

        public override int GetHashCode()
        {
            return (this.GetType().GetHashCode() ^ 93) + this.Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{this.GetType().Name} [Id={this.Id}]";
        }
    }
}