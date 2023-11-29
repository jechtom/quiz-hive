using Microsoft.AspNetCore.SignalR;
using QuizzHive.Server.DataLayer;
using QuizzHive.Server.Hubs;
using QuizzHive.Server.State;

namespace QuizzHive.Server.Services
{
    public class SessionActionDispatcher
    {
        private readonly ILogger<SessionActionDispatcher> logger;
        private readonly IRepository<Session> sessionRepository;
        private readonly IHubContext<AppHub> hub;

        public SessionActionDispatcher(
            ILogger<SessionActionDispatcher> logger,
            IRepository<Session> sessionRepository,
            IHubContext<AppHub> hub)
        {
            this.logger = logger;
            this.sessionRepository = sessionRepository;
            this.hub = hub;
        }

        public async Task<bool> TryDispatchActionAsync(string sessionId, Func<Session, Session> updateAction)
        {
            Session? sessionUpdated;

            for (int retry = 0;; retry++)
            {
                if(retry > 50)
                {
                    // failsafe to prevent infinite loop
                    throw new InvalidOperationException("Can't save. Too many retries.");
                }

                (bool found, Session? sessionOriginal, VersionKey version) = await sessionRepository.TryGetAsync(sessionId);

                if (!found)
                {
                    throw new InvalidOperationException("Session not found.");
                }

                sessionUpdated = updateAction(sessionOriginal ?? throw new InvalidOperationException());

                if (sessionOriginal == sessionUpdated)
                {
                    return false; // no change
                }

                (bool success, VersionKey newVersionKey) = await sessionRepository.TrySetAsync(sessionUpdated.SessionId, sessionUpdated, version);

                if(success)
                {
                    break;
                }
            }

            // distribute new state to connected clients
            foreach (var player in sessionUpdated.Players.Values.Where(p => !p.IsDisconnected))
            {
                var state = sessionUpdated.ResolveStateForClient(player);
                await hub.Clients.Client(player.Id).SendAsync("UpdateStateMessage", state);
            }

            return true;
        }
    }
}
