using Spamma.Api.Web.Infrastructure.Constants;
using Spamma.Api.Web.Infrastructure.Domain.EmailAggregate.Aggregate;
using Spamma.Shared.Tests;

namespace Spamma.Api.Web.Tests.Infrastructure.Domain.EmailAggregate.Aggregate
{
    public class EmailTests
    {
        [Fact]
        public async Task Email_WhenConstructed_ExpectDefault()
        {
            var email = new Email(Guid.NewGuid(), "Test", DateTime.UtcNow, new List<EmailAddress>
            {
                new("address", "name", EmailAddressType.To),
            });

            await Verify(new
            {
                email,
                ListCheck = EntityHelpers.CheckLists(email),
            });
        }

        [Fact]
        public async Task Email_WhenPrivateConstructorCalled_ExpectNewInstance()
        {
            var email = EntityHelpers.CreateEntity<Email>(new
            {
                Id = Guid.NewGuid(),
                Subject = "Test",
                SentDate = DateTime.UtcNow,
                EmailAddresses = new List<EmailAddress>
                {
                    new("address", "name", EmailAddressType.To),
                },
            });

            await Verify(new
            {
                email,
                ListCheck = EntityHelpers.CheckLists(email),
            });
        }
    }
}