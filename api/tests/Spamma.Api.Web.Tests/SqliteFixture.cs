using Microsoft.EntityFrameworkCore;
using Spamma.Api.Web.Infrastructure.Database;

namespace Spamma.Api.Web.Tests
{
    public sealed class SqliteFixture : IDisposable
    {
        private readonly SpammaDataContext _spammaDataContext;

        public SqliteFixture()
        {
            this._spammaDataContext = new SpammaDataContext();
            this._spammaDataContext.Database.EnsureDeleted();
            this._spammaDataContext.Database.Migrate();
        }

        public void Dispose()
        {
            this._spammaDataContext.Database.EnsureDeleted();
            this._spammaDataContext.Dispose();
        }
    }
}