namespace QuizzHive.Server.Services
{
    public record class SessionSegmentProgress
    {
        public required SessionSegment Segment { get; init; }
        public bool HasStarted { get; init; }
        public bool HasEnded { get; init; }
        public required string SegmentId { get; init; }
    }
}