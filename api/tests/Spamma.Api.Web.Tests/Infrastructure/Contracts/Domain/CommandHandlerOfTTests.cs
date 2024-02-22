using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using Spamma.Api.Web.Infrastructure.Contracts.Domain;

namespace Spamma.Api.Web.Tests.Infrastructure.Contracts.Domain
{
    public class CommandHandlerOfTTests
    {
        private readonly StubCommand _command = new();
        private readonly Mock<ILogger<StubCommandHandler>> _logger = new();
        private readonly Mock<IValidator<StubCommand>> _validator = new();

        [Fact]
        public async Task Handle_WhenCommandIsValid_ShouldReturnSuccessResult()
        {
            // Arrange
            this._validator.Setup(x => x.Validate(this._command))
                .Returns(new ValidationResult());
            var handler = new StubCommandHandler(new[] { this._validator.Object }, this._logger.Object);

            // Act
            var result = await handler.Handle(this._command, CancellationToken.None);

            // Assert
            await Verify(new
            {
                result.Status,
                result.Data,
            });
        }

        [Fact]
        public async Task Handle_WhenCommandIsInvalid_ShouldReturnFailedResult()
        {
            // Arrange
            this._validator.Setup(x => x.Validate(this._command))
                .Returns(new ValidationResult(new List<ValidationFailure>
                {
                    new("Property", "Error message"),
                }));
            var handler = new StubCommandHandler(new[] { this._validator.Object }, this._logger.Object);

            // Act
            var result = await handler.Handle(this._command, CancellationToken.None);

            // Assert
            await Verify(new
            {
                result.Status,
                result.ValidationResult,
            });
        }

        public class StubCommandHandler(IEnumerable<IValidator<StubCommand>> validators, ILogger logger)
            : CommandHandler<StubCommand, StubResultData>(validators, logger)
        {
            protected override Task<CommandResult<StubResultData>> HandleInternal(StubCommand request, CancellationToken cancellationToken)
            {
                return Task.FromResult(CommandResult<StubResultData>.Succeeded(new StubResultData
                {
                    Foo = "bar",
                }));
            }
        }

        public class StubCommand : ICommand<StubResultData>;

        public record StubResultData
        {
            public required string Foo { get; init; }
        }
    }
}