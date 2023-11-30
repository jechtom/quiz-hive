namespace QuizHive.Server.Hubs.Messages
{
    public record WrongJoinCodeMessage()
    {
        public required string JoinCode { get; init; }
    }
}
