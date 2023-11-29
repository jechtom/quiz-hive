using System.Collections.Immutable;

namespace QuizzHive.Server.State
{
    public record class Session
    {
        public string SessionId { get; init; } = Guid.NewGuid().ToString("N");
        public string? JoinCode { get; init; }
        public bool HasStarted { get; init; }
        public bool CanConnect { get; init; }
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
            },
            CurrentScreen = this switch
            {
                _ when !player.IsNameSet => "SetName",
                _ when CurrentSegment.Segment is SessionSegmentLobby => "Lobby",
                _ when CurrentSegment.Segment is SessionSegmentQuizz => "Quizz",
                _ => "Wait"
            },
            PlayersCount = Players.Count(p => !p.Value.IsDisconnected && p.Value.IsNameSet),
            Players = Players.Where(p => !p.Value.IsDisconnected && p.Value.IsNameSet).Select(p => new
            {
                p.Value.Name
            }).ToArray(),
            Segment = new
            {
                Id = CurrentSegment.SegmentId,
                CurrentSegment.HasStarted,
                CurrentSegment.HasEnded,
                Type = CurrentSegment.Segment.GetType().Name
            }
        };
    }
}
