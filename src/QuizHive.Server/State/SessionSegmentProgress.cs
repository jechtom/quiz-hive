namespace QuizHive.Server.State
{
    public record class SessionSegmentProgress
    {
        public required SessionSegment Segment { get; init; }
        public bool HasEnded { get; init; }
        public int SegmentId { get; init; }
    }
}