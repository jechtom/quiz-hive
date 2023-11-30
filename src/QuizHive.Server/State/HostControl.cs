namespace QuizHive.Server.State
{
    public record HostControl
    {
        private HostControl(string action, string text)
        {
            Action = action;
            Text = text;
        }

        public string Action { get; init; }
        public string Text { get; init; }


        public static HostControl UnlockSession { get; } = new HostControl("unlock", "Unlock");
        public static HostControl LockSession { get; } = new HostControl("lock", "Lock");
        public static HostControl Start { get; } = new HostControl("start", "Start");
    }
}