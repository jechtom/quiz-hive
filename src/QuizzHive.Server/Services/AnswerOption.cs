namespace QuizzHive.Server.Services
{
    public record class AnswerOption
    {
        public required Content Content { get; init; }
        public bool IsCorrect { get; init; }
    }
}