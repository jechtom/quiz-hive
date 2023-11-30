namespace QuizHive.Server.Hubs.Messages
{
    public class SetAnswerMessage
    {
        public required string[] Answers { get; set; }
    }
}
