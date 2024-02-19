namespace Spamma.TestHarnesses.EmailSender
{
    public record Email
    {
        public string To { get; init; } = string.Empty;

        public string From { get; init; } = string.Empty;

        public string Subject { get; init; } = string.Empty;

        public string Body { get; init; } = string.Empty;
    }
}