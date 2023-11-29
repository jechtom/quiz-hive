namespace QuizzHive.Server.Services
{
    public static class SessionActions
    {
        public static Session TryConnectWithCode(Session session, string playerId, string code)
        {
            code = code?.ToLowerInvariant() ?? string.Empty;

            if(!session.CanConnect || string.IsNullOrEmpty(session.JoinCode))
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
                if (!playerInSession.IsDisconnected)
                {
                    // already connected
                    return session;
                }

                // reconnect
                return session with
                {
                    Players = session.Players.SetItem(playerId, playerInSession with
                    {
                        IsDisconnected = false
                    })
                };
            }

            // add new player
            return session with
            {
                Players = session.Players.Add(playerId, new PlayerInSession()
                {
                    Id = playerId,
                    Name = string.Empty
                })
            };
        }

        public static Session RemovePlayer(Session session, string playerId)
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
                    IsDisconnected = true
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

            if(playerInSession.IsNameSet)
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
                || session.CurrentSegment.SegmentId != segmentId)
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
    }
}
