using FluentValidation.Results;
using Spamma.Api.Web.Infrastructure.Constants;
using Spamma.Api.Web.Infrastructure.Contracts.Domain;

namespace Spamma.Api.Web.Tests.Infrastructure.Contracts.Domain
{
    public class CommandResultOfTTests
    {
        [Fact]
        public async Task ErrorData_WhenStatusIsNotFailed_ThrowsInvalidOperationException()
        {
            var commandResult = CommandResult<ResultData>.Succeeded(new ResultData
            {
                Foo = "bar",
            });

            var exception = Assert.Throws<InvalidOperationException>(() => commandResult.ErrorData);

            await Verify(exception.Message);
        }

        [Fact]
        public async Task ErrorData_WhenStatusIsFailed_ExpectErrorData()
        {
            var commandResult = CommandResult<ResultData>.Failed(new ErrorData(ErrorCode.SavingChanges));

            await Verify(new
            {
                commandResult.Status,
                commandResult.ErrorData,
            });
        }

        [Fact]
        public async Task ValidationResult_WhenStatusIsNotInvalid_ThrowsInvalidOperationException()
        {
            var commandResult = CommandResult<ResultData>.Succeeded(new ResultData
            {
                Foo = "bar",
            });

            var exception = Assert.Throws<InvalidOperationException>(() => commandResult.ValidationResult);

            await Verify(exception.Message);
        }

        [Fact]
        public async Task ValidationResult_WhenStatusIsInvalid_ExpectValidationResult()
        {
            var commandResult = CommandResult<ResultData>.Invalid(new ValidationResult(new List<ValidationFailure>
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
        public async Task Data_WhenStatusIsSucceeded_ExpectData()
        {
            var commandResult = CommandResult<ResultData>.Succeeded(new ResultData
            {
                Foo = "bar",
            });

            await Verify(new
            {
                commandResult.Data,
                commandResult.Status,
            });
        }

        [Fact]
        public async Task Data_WhenStatusIsNotSucceeded_ThrowsInvalidOperationException()
        {
            var commandResult = CommandResult<ResultData>.Failed(new ErrorData(ErrorCode.SavingChanges));

            var exception = Assert.Throws<InvalidOperationException>(() => commandResult.ValidationResult);

            await Verify(exception.Message);
        }

        private record ResultData
        {
            public required string Foo { get; init; }
        }
    }
}