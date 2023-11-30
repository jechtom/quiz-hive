using QuizHive.Server.Services;

namespace QuizHive.Server.State
{
    public record class AnswerOption
    {
        public required Content Content { get; init; }
        public bool IsCorrect { get; init; }
    }
}