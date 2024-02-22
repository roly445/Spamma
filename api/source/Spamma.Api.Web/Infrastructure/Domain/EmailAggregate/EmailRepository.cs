using Spamma.Api.Web.Infrastructure.Contracts.Domain;
using Spamma.Api.Web.Infrastructure.Database;
using Spamma.Api.Web.Infrastructure.Domain.EmailAggregate.Aggregate;

namespace Spamma.Api.Web.Infrastructure.Domain.EmailAggregate
{
    public class EmailRepository : Repository<Email, Email>
    {
        public EmailRepository(SpammaDataContext dbContext)
            : base(dbContext)
        {
        }
    }
}