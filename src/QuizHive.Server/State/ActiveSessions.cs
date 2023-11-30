using System.Collections.Immutable;

namespace QuizHive.Server.State
{
    public record class ActiveSessions
    {
        private ActiveSessions()
        {
        }

        /// <summary>
        /// Gets map of connect codes to session Ids.
        /// </summary>
        public ImmutableDictionary<string, string> ConnectCodesToSessionId { get; init; }
            = ImmutableDictionary<string, string>.Empty.WithComparers(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets map of action session Ids.
        /// </summary>
        public ImmutableHashSet<string> ActiveSessionsIds { get; init; }
            = ImmutableHashSet<string>.Empty.WithComparer(StringComparer.OrdinalIgnoreCase);


        public static ActiveSessions Empty { get; } = new ActiveSessions();

        public ActiveSessions WithAddSession(string sessionId, string connectCode)
        {
            if (ActiveSessionsIds.Contains(sessionId))
            {
                throw new InvalidOperationException("Session with given Id already exists.");
            }

            if (ConnectCodesToSessionId.ContainsKey(connectCode))
            {
                throw new InvalidOperationException("Session with given connect code already exists.");
            }

            return this with
            {
                ActiveSessionsIds = ActiveSessionsIds.Add(sessionId),
                ConnectCodesToSessionId = ConnectCodesToSessionId.Add(connectCode, sessionId)
            };
        }

        public ActiveSessions WithoutRemoveSession(string sessionId)
        {
            if (!ActiveSessionsIds.Contains(sessionId))
            {
                return this;
            }

            var codesWithSessionId = ConnectCodesToSessionId
                .Where(cs => cs.Value.Equals(sessionId, StringComparison.OrdinalIgnoreCase)) // all values that references this session
                .Select(cs => cs.Key); // take the code

            return this with
            {
                ActiveSessionsIds = ActiveSessionsIds.Remove(sessionId),
                ConnectCodesToSessionId = ConnectCodesToSessionId.RemoveRange(codesWithSessionId)
            };
        }
    }
}
