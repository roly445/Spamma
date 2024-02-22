using FluentValidation.Results;
using Spamma.Api.Web.Infrastructure.Constants;

namespace Spamma.Api.Web.Infrastructure.Contracts.Domain
{
    public class CommandResult<T> : CommandResult
    {
        private readonly T? _data;

        private CommandResult(ErrorData errorData)
            : base(errorData)
        {
        }

        private CommandResult(T data)
            : base()
        {
            this._data = data;
        }

        private CommandResult(ValidationResult validationResult)
            : base(validationResult)
        {
        }

        public T Data
        {
            get
            {
                if (this.Status != CommandResultStatus.Succeeded)
                {
                    throw new System.InvalidOperationException("Data is only available when the status is Succeeded");
                }

                return this._data!;
            }
        }

        public new static CommandResult<T> Invalid(ValidationResult validationResult)
        {
            return new CommandResult<T>(validationResult);
        }

        public new static CommandResult<T> Failed(ErrorData errorData)
        {
            return new CommandResult<T>(errorData);
        }

        public static CommandResult<T> Succeeded(T data)
        {
            return new CommandResult<T>(data);
        }
    }
}