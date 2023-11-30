using System.Collections.Immutable;
using System.Numerics;

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
            QuizSegment = ResolveQuizSegment(player),
            CurrentScreen = this switch
            {
                _ when !player.IsNameSet => "EnterName",
                _ when CurrentSegment.Segment is SessionSegmentLobby && player.IsHost => "Lobby",
                _ when CurrentSegment.Segment is SessionSegmentQuiz && player.IsHost => "Quiz",
                _ when CurrentSegment.Segment is SessionSegmentQuiz && player.HasAnswered => "WaitForOthers",
                _ when CurrentSegment.Segment is SessionSegmentQuiz => "Quiz",
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
                CurrentSegment.HasEnded,
                Type = CurrentSegment.Segment.GetType().Name
            }
        };

        public static bool IsActivePlayer(PlayerInSession p)
            => !p.IsDisconnected && p.IsNameSet && !p.IsHost;

        public bool CanStart 
            => Players.Values.Any(IsActivePlayer) && CurrentSegment.Segment is SessionSegmentLobby;

        public bool CanChangeLock // 
            => CurrentSegment.Segment is not SessionSegmentEnd;

        private ImmutableList<HostControl> ResolveHostControls()
        {
            var list = ImmutableList<HostControl>.Empty;
            if (CanChangeLock)
            {
                list = list.Add(IsUnlocked ? HostControl.LockSession : HostControl.UnlockSession);
            };
            
            switch (CurrentSegment.Segment)
            {
                case SessionSegmentLobby lobby when Players.Values.Any(IsActivePlayer):
                    list = list.Add(HostControl.Continue);
                    break;
                case SessionSegmentQuiz q when !CurrentSegment.HasEnded:
                    list = list.Add(HostControl.EndRound);
                    break;
                case SessionSegmentQuiz q when CurrentSegment.HasEnded:
                    list = list.Add(HostControl.Continue);
                    break;
            }

            return list;
        }

        private object ResolveQuizSegment(PlayerInSession player)
        {
            return new
            {
                RemainToAnswer = player.IsHost ? Players.Values.Count(p => IsActivePlayer(p) && !p.HasAnswered) : -1,
                QuestionText = CurrentSegment.Segment switch
                {
                    SessionSegmentQuiz q when player.IsHost => q.QuestionContent.Text,
                    _ => ""
                },
                Answers = CurrentSegment.Segment switch
                {
                    SessionSegmentQuiz q => q.AnswerOptions
                        .Select((a, i) => new { 
                            Text = a.Content.Text, 
                            Id = i.ToString(),
                            IsCorrect = player.IsHost && CurrentSegment.HasEnded && a.IsCorrect
                        }).ToArray(),
                    _ => []
                },
                ShowAnswers = CurrentSegment.Segment switch
                {
                    SessionSegmentQuiz q when CurrentSegment.HasEnded => true,
                    _ => false
                },
            };
        }
    }
}
