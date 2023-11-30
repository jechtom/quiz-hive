using System.Collections.Immutable;

namespace QuizHive.Server.State
{
    public record class Session
    {
        public string SessionId { get; init; } = Guid.NewGuid().ToString("N");
        public string? JoinCode { get; init; }
        public bool HasStarted { get; init; }
        public bool IsUnlocked { get; init; }
        public IImmutableList<SessionSegment> Segments { get; init; }
            = ImmutableList<SessionSegment>.Empty;
        public required SessionSegmentProgress CurrentSegment { get; init; }
        public IImmutableDictionary<string, PlayerInSession> Players { get; init; }
            = ImmutableDictionary<string, PlayerInSession>.Empty;

        public object ResolveStateForClient(PlayerInSession player) => new
        {
            Me = new
            {
                player.IsNameSet,
                player.Name,
                player.IsHost,
                HostControls = player.IsHost ? ResolveHostControls() : ImmutableList<HostControl>.Empty
            },
            JoinCode = IsUnlocked ? JoinCode : string.Empty,
            IsUnlocked = IsUnlocked,
            CurrentScreen = this switch
            {
                _ when !player.IsNameSet => "EnterName",
                _ when CurrentSegment.Segment is SessionSegmentLobby && player.IsHost => "Lobby",
                _ when CurrentSegment.Segment is SessionSegmentQuiz && player.IsHost => "Quiz",
                _ when CurrentSegment.Segment is SessionSegmentQuiz => "QuizAnswer",
                _ when CurrentSegment.Segment is SessionSegmentEnd => "End",
                _ => "Wait"
            },
            PlayersCount = Players.Values.Count(IsActivePlayer),
            Players = Players.Values.Where(IsActivePlayer).Select(p => new
            {
                p.Name
            }).ToArray(),
            Segment = new
            {
                Id = CurrentSegment.SegmentId,
                CurrentSegment.HasStarted,
                CurrentSegment.HasEnded,
                Type = CurrentSegment.Segment.GetType().Name
            }
        };

        private static bool IsActivePlayer(PlayerInSession p)
            => !p.IsDisconnected && p.IsNameSet && !p.IsHost;

        public bool CanStart 
            => Players.Values.Any(IsActivePlayer) && CurrentSegment.Segment is SessionSegmentLobby;

        private ImmutableList<HostControl> ResolveHostControls()
        {
            switch (CurrentSegment.Segment)
            {
                case SessionSegmentLobby lobby:
                    return [ 
                            (IsUnlocked ? HostControl.LockSession : HostControl.UnlockSession),
                            HostControl.Start
                        ];
                default:
                    return ImmutableList<HostControl>.Empty;
            }
        }
    }
}
