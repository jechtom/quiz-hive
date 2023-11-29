namespace QuizzHive.Server.Models
{
    public class SessionJoinResponse
    {
        public bool Success { get; set; }
        public required string Token { get; set; }
    }
}
