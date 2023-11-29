using QuizzHive.Server.Services;

namespace QuizzHive.Server.State
{
    public record class AnswerOption
    {
        public required Content Content { get; init; }
        public bool IsCorrect { get; init; }
    }
}