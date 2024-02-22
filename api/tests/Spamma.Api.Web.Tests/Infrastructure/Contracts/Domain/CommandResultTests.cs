using FluentValidation.Results;
using Spamma.Api.Web.Infrastructure.Constants;
using Spamma.Api.Web.Infrastructure.Contracts.Domain;

namespace Spamma.Api.Web.Tests.Infrastructure.Contracts.Domain
{
    public class CommandResultTests
    {
        [Fact]
        public async Task ErrorData_WhenStatusIsNotFailed_ThrowsInvalidOperationException()
        {
            var commandResult = CommandResult.Succeeded();

            var exception = Assert.Throws<InvalidOperationException>(() => commandResult.ErrorData);

            await Verify(exception.Message);
        }

        [Fact]
        public async Task ErrorData_WhenStatusIsFailed_ExpectErrorData()
        {
            var commandResult = CommandResult.Failed(new ErrorData(ErrorCode.SavingChanges));

            await Verify(new
            {
                commandResult.Status,
                commandResult.ErrorData,
            });
        }

        [Fact]
        public async Task ValidationResult_WhenStatusIsNotInvalid_ThrowsInvalidOperationException()
        {
            var commandResult = CommandResult.Succeeded();

            var exception = Assert.Throws<InvalidOperationException>(() => commandResult.ValidationResult);

            await Verify(exception.Message);
        }

        [Fact]
        public async Task ValidationResult_WhenStatusIsInvalid_ExpectValidationResult()
        {
            var commandResult = CommandResult.Invalid(new ValidationResult(new List<ValidationFailure>
            {
                new("Property", "Error message"),
            }));

            await Verify(new
            {
                commandResult.Status,
                commandResult.ValidationResult,
            });
        }

        [Fact]
        public async Task Status_WhenConstructed_ExpectSucceeded()
        {
            var commandResult = CommandResult.Succeeded();

            await Verify(commandResult.Status);
        }
    }
}