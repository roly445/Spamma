using System.Diagnostics.CodeAnalysis;
using Spamma.Api.Web.Infrastructure.Contracts.Domain;

namespace Spamma.Api.Web.Infrastructure.Domain.EmailAggregate.Aggregate
{
    public class Email : Entity, IAggregateRoot
    {
        private readonly List<EmailAddress> _emailAddresses = new();

        public Email(Guid id, string subject, DateTime sentDate, IReadOnlyList<EmailAddress> emailAddresses)
            : base(id)
        {
            this.Subject = subject;
            this.SentDate = sentDate;
            this._emailAddresses = emailAddresses.ToList();
        }

        [SuppressMessage("SonarAnalyzer.CSharp", "CS8618", Justification = "Constructor used for ef core purposes only.")]
        private Email()
        {
        }

        public string Subject { get; private set; } = string.Empty;

        public DateTime SentDate { get; private set; }

        public IReadOnlyList<EmailAddress> EmailAddresses => this._emailAddresses;
    }
}