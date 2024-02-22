using MimeKit;
using ResultMonad;
using Spamma.Api.Web.Infrastructure.Contracts.MessageHandling;
using Spamma.Api.Web.Infrastructure.Contracts.SutWrappers;

namespace Spamma.Api.Web.Infrastructure.MessageHandling
{
    public class LocalMessageStoreProvider : IMessageStoreProvider
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<LocalMessageStoreProvider> _logger;
        private readonly IDirectoryWrapper _directoryWrapper;
        private readonly IFileWrapper _fileWrapper;

        public LocalMessageStoreProvider(
            IHostEnvironment hostEnvironment, ILogger<LocalMessageStoreProvider> logger,
            IDirectoryWrapper directoryWrapper, IFileWrapper fileWrapper)
        {
            this._hostEnvironment = hostEnvironment;
            this._logger = logger;
            this._directoryWrapper = directoryWrapper;
            this._fileWrapper = fileWrapper;
        }

        public async ValueTask<Result> StoreMessageContentAsync(Guid messageId, MimeMessage messageContent, CancellationToken cancellationToken = default)
        {
            var path = Path.Combine(this._hostEnvironment.ContentRootPath, "messages");
            if (!this._directoryWrapper.Exists(path))
            {
                try
                {
                    this._directoryWrapper.CreateDirectory(path);
                }
                catch (Exception e)
                {
                    if (e is not (UnauthorizedAccessException or PathTooLongException
                        or DirectoryNotFoundException))
                    {
                        throw;
                    }

                    this._logger.LogError(e, "Failed to create directory for message storage.");
                    return Result.Fail();
                }
            }

            await messageContent.WriteToAsync(Path.Combine(path, $"{messageId}.eml"), cancellationToken);
            return Result.Ok();
        }

        public ValueTask<Result> DeleteMessageContentAsync(Guid messageId, CancellationToken cancellationToken = default)
        {
            var path = Path.Combine(this._hostEnvironment.ContentRootPath, "messages");
            if (!this._directoryWrapper.Exists(path))
            {
                return new ValueTask<Result>(Result.Fail());
            }

            this._fileWrapper.Delete(Path.Combine(path, $"{messageId}.eml"));
            return new ValueTask<Result>(Result.Ok());
        }
    }
}