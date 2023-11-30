using System.Runtime.CompilerServices;

namespace QuizHive.Server.State
{
    public static class SessionActions
    {
        public static Session TryConnectWithCode(Session session, string playerId, string code)
        {
            code = code?.ToLowerInvariant() ?? string.Empty;

            if (!session.IsUnlocked || string.IsNullOrEmpty(session.JoinCode))
            {
                // this session don't allow new players
                return session;
            }

            if (!string.Equals(session.JoinCode, code, StringComparison.OrdinalIgnoreCase))
            {
                // incorrect code
                return session;
            }

            if (session.Players.TryGetValue(playerId, out PlayerInSession? playerInSession))
            {
                // already connected
                return session;
            }

            // add new player
            bool isHost = session.Players.Count == 0; // first player is the host
            return session with
            {
                Players = session.Players.Add(playerId, new PlayerInSession()
                {
                    Id = playerId,
                    Name = string.Empty,
                    IsHost = isHost,
                    IsNameSet = isHost // host don't need to set name
                })
            };
        }

        public static Session TryReconnect(Session session, string newPlayerId, string reconnectCode, out string? oldPlayerId)
        {
            var player = session.Players.Values.SingleOrDefault(p => p.ReconnectCode == reconnectCode);

            if(player == null)
            {
                // no player with given reconnect code
                oldPlayerId = null;
                return session;
            }

            // send old player Id to send disconnect
            oldPlayerId = player.Id;

            // reconnect
            return session with
            {
                Players = session.Players
                    .Remove(player.Id)
                    .Add(newPlayerId, player with
                    {
                        Id = newPlayerId,
                        IsDisconnected = false
                    })
            };
        }

        public static Session RemovePlayer(Session session, string playerId, bool invalidateReconnect)
        {
            if (!session.Players.TryGetValue(playerId, out PlayerInSession? playerInSession))
            {
                // player is not in the session
                return session;
            }

            // remove player
            return session with
            {
                Players = session.Players.SetItem(playerId, playerInSession with
                {
                    IsDisconnected = true,
                    ReconnectCode = invalidateReconnect ? Guid.NewGuid().ToString("N") : playerInSession.ReconnectCode
                })
            };
        }

        public static Session PlayerSetName(Session session, string playerId, string name)
        {
            if (!session.Players.TryGetValue(playerId, out PlayerInSession? playerInSession))
            {
                // player is not in the session
                return session;
            }

            if (playerInSession.IsNameSet)
            {
                // name already set
                return session;
            }

            // set name
            return session with
            {
                Players = session.Players.SetItem(playerId, playerInSession with
                {
                    IsNameSet = true,
                    Name = name
                })
            };
        }

        public static Session AnswerSegment(Session session, string playerId, string segmentId, string answerCode)
        {
            if (session.CurrentSegment == null
                || session.CurrentSegment.SegmentId.ToString() != segmentId)
            {
                // session segment does not match
                return session;
            }

            if (!session.Players.TryGetValue(playerId, out PlayerInSession? playerInSession))
            {
                // player is not in the session
                return session;
            }

            if (playerInSession.HasAnswered)
            {
                // already answered
                return session;
            }

            if (!session.CurrentSegment.HasStarted || session.CurrentSegment.HasEnded)
            {
                // not started or already ended
                return session;
            }

            // answer!
            return session with
            {
                Players = session.Players.SetItem(playerId, playerInSession with
                {
                    HasAnswered = true,
                    AnswerCode = answerCode
                })
            };
        }

        public static Session EnterCommandAction(Session session, string playerId, string action)
        {

            if (!session.Players.TryGetValue(playerId, out PlayerInSession? playerInSession))
            {
                // player is not in the session
                return session;
            }

            if(!playerInSession.IsHost)
            {
                // not a host
                return session;
            }

            switch (action)
            {
                case string a when 
                    a == HostControl.UnlockSession.Action && session.CanChangeLock && !session.IsUnlocked:
                    return session with
                    {
                        IsUnlocked = true
                    };
                case string a when
                    a == HostControl.LockSession.Action && session.CanChangeLock && session.IsUnlocked:
                    return session with
                    {
                        IsUnlocked = false
                    };
                case string a when a == HostControl.Start.Action:
                    return StartSession(session);
                default:
                    return session;
            }
        }

        private static Session StartSession(Session session)
        {
            if(!session.CanStart)
            {
                return session;
            }

            var nextSegmentId = session.CurrentSegment.SegmentId + 1;
            var nextSegment = session.Segments[nextSegmentId];

            return session with
            {
                IsUnlocked = session.IsUnlocked && (nextSegment is not SessionSegmentEnd /*lock on end segment*/),
                CurrentSegment = new SessionSegmentProgress()
                {
                    SegmentId = nextSegmentId,
                    Segment = nextSegment,
                    HasStarted = false,
                    HasEnded = false
                }
            };
        }
    }
}
