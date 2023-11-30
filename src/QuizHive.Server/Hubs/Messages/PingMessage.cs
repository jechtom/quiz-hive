namespace QuizHive.Server.Hubs.Messages
{
    public record PingMessage()
    {
        public Guid Id { get; } = Guid.NewGuid();
    }
}
