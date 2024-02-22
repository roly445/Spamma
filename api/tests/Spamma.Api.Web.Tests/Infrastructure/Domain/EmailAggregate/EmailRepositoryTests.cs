using System.Linq.Expressions;
using Spamma.Api.Web.Infrastructure.Constants;
using Spamma.Api.Web.Infrastructure.Contracts.Domain;
using Spamma.Api.Web.Infrastructure.Domain.EmailAggregate;
using Spamma.Api.Web.Infrastructure.Domain.EmailAggregate.Aggregate;

namespace Spamma.Api.Web.Tests.Infrastructure.Domain.EmailAggregate
{
    [Collection("Sqlite collection")]
    public class EmailRepositoryTests : RepositoryTests<Email, EmailRepository>
    {
        protected override Specification<Email> FindValidSpec => new BySubjectSpecification("Test 1");

        protected override Specification<Email> FindMultipleValidSpec => new BySubjectSpecification("Test 2");

        protected override Specification<Email> FindInvalidSpec => new BySubjectSpecification("no-test");

        protected override Specification<Email> FindMultipleInvalidSpec => new BySubjectSpecification("no-test");

        protected override Email GenerateSeedEntity() =>
            new(
                Guid.NewGuid(),
                "Test",
                DateTime.Now,
                new List<EmailAddress>
                {
                    new("address", "name", EmailAddressType.To),
                });

        protected override IReadOnlyList<Email> GenerateSeedEntities() => new List<Email>
        {
            new(
                Guid.NewGuid(),
                "Test 1",
                DateTime.Now,
                new List<EmailAddress>
                {
                    new("address 1", "name 1", EmailAddressType.To),
                }),
            new(
                Guid.NewGuid(),
                "Test 2",
                DateTime.Now,
                new List<EmailAddress>
                {
                    new("address 2", "name 2", EmailAddressType.To),
                }),
            new(
                Guid.NewGuid(),
                "Test 2",
                DateTime.Now,
                new List<EmailAddress>
                {
                    new("address 2", "name 2", EmailAddressType.To),
                }),
        };

        private class BySubjectSpecification(string subject) : Specification<Email>
        {
            public override Expression<Func<Email, bool>> ToExpression()
            {
                return x => x.Subject == subject;
            }
        }
    }
}