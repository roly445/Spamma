using FluentValidation.Results;
using Spamma.Api.Web.Infrastructure.Constants;

namespace Spamma.Api.Web.Infrastructure.Contracts.Domain
{
    public class CommandResult
    {
        private readonly ErrorData? _errorData;
        private readonly ValidationResult? _validationResult;

        protected CommandResult()
        {
            this.Status = CommandResultStatus.Succeeded;
        }

        protected CommandResult(ErrorData errorData)
        {
            this.Status = CommandResultStatus.Failed;
            this._errorData = errorData;
        }

        protected CommandResult(ValidationResult validationResult)
        {
            this.Status = CommandResultStatus.Invalid;
            this._validationResult = validationResult;
        }

        public ErrorData ErrorData
        {
            get
            {
                if (this.Status != CommandResultStatus.Failed)
                {
                    throw new System.InvalidOperationException("ErrorData is only available when the status is Failed");
                }

                return this._errorData!;
            }
        }

        public ValidationResult ValidationResult
        {
            get
            {
                if (this.Status != CommandResultStatus.Invalid)
                {
                    throw new System.InvalidOperationException("ValidationResult is only available when the status is Invalid");
                }

                return this._validationResult!;
            }
        }

        public CommandResultStatus Status { get; protected set; }

        public static CommandResult Invalid(ValidationResult validationResult)
        {
            return new CommandResult(validationResult);
        }

        public static CommandResult Failed(ErrorData errorData)
        {
            return new CommandResult(errorData);
        }

        public static CommandResult Succeeded()
        {
            return new CommandResult();
        }
    }
}