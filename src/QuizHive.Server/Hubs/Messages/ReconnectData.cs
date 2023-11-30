namespace QuizHive.Server.Hubs.Messages
{
    public record EnterReconnectMessage(string SessionId, string ReconnectCode);
}
