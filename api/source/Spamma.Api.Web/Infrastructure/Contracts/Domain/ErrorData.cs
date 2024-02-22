using Spamma.Api.Web.Infrastructure.Constants;

namespace Spamma.Api.Web.Infrastructure.Contracts.Domain
{
    public sealed class ErrorData(ErrorCode code, string message)
    {
        public ErrorData(ErrorCode code)
            : this(code, code.ToString())
        {
        }

        public ErrorCode Code { get; } = code;

        public string Message { get; } = message;
    }
}