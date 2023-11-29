namespace QuizzHive.Server.Hubs.Messages
{
    public record PingMessage()
    {
        public Guid Id { get; } = Guid.NewGuid();
    }
}
