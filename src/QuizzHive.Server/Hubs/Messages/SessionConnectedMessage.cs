namespace QuizzHive.Server.Hubs.Messages
{
    public record SessionConnectedMessage(string SessionId, string ReconnectCode);
}
