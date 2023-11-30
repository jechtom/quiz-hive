namespace QuizHive.Server.Hubs.Messages
{
    public static class MessageCodes
    {
        public const string SessionEnd = "SessionEnd";
        public const string SessionKick = "SessionKick";
        public const string InvalidJoinCode = "InvalidJoinCode";
        public const string SessionStateUpdate = "Update";
        public const string SessionConnect = "SessionConnect";
        public const string SessionLeave = "SessionLeave";
    }
}
