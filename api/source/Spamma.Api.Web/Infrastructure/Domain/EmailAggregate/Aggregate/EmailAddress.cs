using System.Diagnostics.CodeAnalysis;
using Spamma.Api.Web.Infrastructure.Constants;
using Spamma.Api.Web.Infrastructure.Contracts.Domain;

namespace Spamma.Api.Web.Infrastructure.Domain.EmailAggregate.Aggregate
{
    public class EmailAddress : Entity
    {
        public EmailAddress(string address, string name, EmailAddressType emailAddressType)
            : base(Guid.NewGuid())
        {
            this.Address = address;
            this.Name = name;
            this.EmailAddressType = emailAddressType;
        }

        [SuppressMessage("SonarAnalyzer.CSharp", "CS8618", Justification = "Constructor used for ef core purposes only.")]
        private EmailAddress()
        {
        }

        public string Address { get; private set; } = string.Empty;

        public string Name { get; private set; } = string.Empty;

        public EmailAddressType EmailAddressType { get; private set; }
    }
}