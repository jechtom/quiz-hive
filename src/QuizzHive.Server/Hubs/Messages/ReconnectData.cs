namespace QuizzHive.Server.Hubs.Messages
{
    public record EnterReconnectMessage(string SessionId, string ReconnectCode);
}
