using Spamma.Api.Web.Infrastructure.Constants;
using Spamma.Api.Web.Infrastructure.Domain.EmailAggregate.Aggregate;
using Spamma.Shared.Tests;

namespace Spamma.Api.Web.Tests.Infrastructure.Domain.EmailAggregate.Aggregate
{
    public class EmailAddressTests
    {
        [Fact]
        public async Task Email_WhenConstructed_ExpectDefault()
        {
            var emailAddress = new EmailAddress("address", "name", EmailAddressType.To);

            await Verify(new
            {
                emailAddress,
                ListCheck = EntityHelpers.CheckLists(emailAddress),
            });
        }

        [Fact]
        public async Task Email_WhenPrivateConstructorCalled_ExpectNewInstance()
        {
            var emailAddress = EntityHelpers.CreateEntity<EmailAddress>(new
            {
                Id = Guid.NewGuid(),
                Address = "address",
                Name = "name",
            });

            await Verify(new
            {
                emailAddress,
                ListCheck = EntityHelpers.CheckLists(emailAddress),
            });
        }
    }
}