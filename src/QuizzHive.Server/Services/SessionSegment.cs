using System.Collections.Immutable;

namespace QuizzHive.Server.Services
{
    public abstract record class SessionSegment
    {

    }

    public record class SessionSegmentLobby : SessionSegment
    {

    }

    public record class SessionSegmentEnd : SessionSegment
    {

    }

    public record class SessionSegmentQuizz : SessionSegment
    {
        public required Content QuestionContent { get; init; }
        public bool CanSelectedMultiple { get; init; } = false;
        public required IImmutableList<AnswerOption> AnswerOptions { get; init; }
    }
}