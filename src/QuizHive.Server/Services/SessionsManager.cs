using QuizHive.Server.DataLayer;
using QuizHive.Server.State;

namespace QuizHive.Server.Services
{
    public class SessionsManager
    {
        private readonly IRepository<ActiveSessions> activeSessionsRepo;
        private readonly IRepository<Session> sessionsRepo;

        public SessionsManager(IRepository<ActiveSessions> activeSessionsRepo, IRepository<Session> sessionsRepo)
        {
            this.activeSessionsRepo = activeSessionsRepo;
            this.sessionsRepo = sessionsRepo;
        }

        public async Task<(Session?, VersionKey)> GetSessionByIdAsync(string id)
            => await sessionsRepo.TryGetAsync(id) switch
            {
                (false, _, _) => (null, default), // not found
                (true, var session, var version) => (session ?? throw new NullReferenceException(), version)
            };

        public async Task<(ActiveSessions, VersionKey)> GetActiveSessionsAsync()
            => await activeSessionsRepo.TryGetAsync(string.Empty) switch
            {
                (false, _, _) => (ActiveSessions.Empty, VersionKey.NonExisting), // not found
                (true, var activeSessions, var version) => (activeSessions ?? throw new NullReferenceException(), version)
            };

        public async Task<string?> GetSessionIdByJoinCodeAsync(string joinCode)
        {
            // try find session Id in list of active sessions by join code
            (var sessions, _) = await GetActiveSessionsAsync();
            if(!sessions.ConnectCodesToSessionId.TryGetValue(joinCode, out var sessionId))
            {
                return default;
            }

            return sessionId;
        }
    }
}

