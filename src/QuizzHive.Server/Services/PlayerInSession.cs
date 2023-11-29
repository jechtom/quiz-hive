namespace QuizzHive.Server.Services
{
    public record PlayerInSession
    {
        public required string Id { get; init; }
        public string ReconnectCode { get; init; } = Guid.NewGuid().ToString("N");
        public required string Name { get; init; }
        public int TotalScore { get; init; }
        public bool HasAnswered { get; init; }
        public string? AnswerCode { get; init; }
        public bool IsDisconnected { get; init; }
        public bool IsNameSet { get; init; }
    }
}