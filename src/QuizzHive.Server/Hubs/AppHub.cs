using Microsoft.AspNetCore.SignalR;
using QuizzHive.Server.DataLayer;
using QuizzHive.Server.Hubs.Messages;
using QuizzHive.Server.Services;
using QuizzHive.Server.State;
using System.Linq;
using System.Text.RegularExpressions;

namespace QuizzHive.Server.Hubs
{
    public class AppHub : Hub
    {
        private readonly ILogger<AppHub> logger;
        private readonly ClientsManager clientsManager;
        private readonly SessionActionDispatcher sessionActionDispatcher;
        private readonly SessionsManager sessionsManager;

        public AppHub(ILogger<AppHub> logger, ClientsManager clientsManager, 
            SessionActionDispatcher sessionActionDispatcher,
            SessionsManager sessionsManager)
        {
            this.logger = logger;
            this.clientsManager = clientsManager;
            this.sessionActionDispatcher = sessionActionDispatcher;
            this.sessionsManager = sessionsManager;
        }

        public override Task OnConnectedAsync()
        {
            clientsManager.OnClientConnected(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            clientsManager.OnClientDisconnected(Context.ConnectionId, out string? linkedSessionId);

            if (linkedSessionId != null)
            {
                // remove player from the session
                await sessionActionDispatcher.TryDispatchActionAsync(
                    linkedSessionId,
                    s => SessionActions.RemovePlayer(s, Context.ConnectionId)
                );
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task EnterReconnectMessage(EnterReconnectMessage data)
        {
            logger.LogDebug("Reconnect for: {SessionId}", data.SessionId);

            (var activeSessions, _)= await sessionsManager.GetActiveSessionsAsync();
            if(!activeSessions.ActiveSessionsIds.Contains(data.SessionId))
            {
                // not active anymore
                return;
            }

            if (!clientsManager.TryAssignSessionLink(Context.ConnectionId, data.SessionId))
            {
                logger.LogWarning("Failed to join assign session link. Probably client disconnected or already assigned.");
                return;
            }

            bool success = false;
            try
            {
                // try add player to the session
                (success, Session? session) = await sessionActionDispatcher.TryDispatchActionAsync(
                    data.SessionId,
                    s => SessionActions.TryReconnect(s, Context.ConnectionId, data.ReconnectCode)
                );

                if (success)
                {
                    session = session ?? throw new InvalidOperationException("Unexpected null.");
                    await Clients.Client(Context.ConnectionId)
                        .SendAsync(nameof(Messages.SessionConnectedMessage),
                            new SessionConnectedMessage(
                                session.SessionId,
                                session.Players[Context.ConnectionId].ReconnectCode
                        ));
                }
            }
            finally
            {
                if (!success)
                {
                    clientsManager.RemoveSessionLink(Context.ConnectionId, data.SessionId);
                }
            }
        }

        public async Task EnterCodeMessage(string code)
        {
            logger.LogDebug("Code from client: {Code}", code);

            string? sessionId = await sessionsManager.GetSessionIdByJoinCodeAsync(code);

            if(sessionId == null) 
            {
                // session not found
                await Clients.Client(Context.ConnectionId)
                    .SendAsync(nameof(Messages.WrongJoinCodeMessage), new Messages.WrongJoinCodeMessage() { JoinCode = code });
                return;
            }

            if(!clientsManager.TryAssignSessionLink(Context.ConnectionId, sessionId))
            {
                logger.LogWarning("Failed to join assign session link. Probably client disconnected or already assigned.");
                return;
            }

            bool success = false;
            try
            {
                // try add player to the session
                (success, Session? session) = await sessionActionDispatcher.TryDispatchActionAsync(
                    sessionId,
                    s => SessionActions.TryConnectWithCode(s, Context.ConnectionId, code)
                );

                if (success)
                {
                    session = session ?? throw new InvalidOperationException("Unexpected null.");
                    await Clients.Client(Context.ConnectionId)
                        .SendAsync(nameof(Messages.SessionConnectedMessage), 
                            new Messages.SessionConnectedMessage(
                                session.SessionId,
                                session.Players[Context.ConnectionId].ReconnectCode
                        ));
                }
                else
                {
                    // can't connect
                    await Clients.Client(Context.ConnectionId)
                        .SendAsync(nameof(Messages.WrongJoinCodeMessage), new Messages.WrongJoinCodeMessage() { JoinCode = code });
                    return;
                }
            }
            finally
            {
                if (!success)
                {
                    clientsManager.RemoveSessionLink(Context.ConnectionId, sessionId);
                }
            }
        }

        public async Task EnterNameMessage(string name)
        {
            logger.LogInformation("Name from client: {Name}", name);
            if(!clientsManager.TryGetSessionLink(Context.ConnectionId, out string? linkedSessionId))
            {
                return;
            }

            await sessionActionDispatcher.TryDispatchActionAsync(
                linkedSessionId,
                s => SessionActions.PlayerSetName(s, Context.ConnectionId, name)
            );
        }

        public async Task EnterCommandMessage(string action)
        {
            logger.LogInformation("Action from client: {Action}", action);
            if (!clientsManager.TryGetSessionLink(Context.ConnectionId, out string? linkedSessionId))
            {
                return;
            }

            await sessionActionDispatcher.TryDispatchActionAsync(
                linkedSessionId,
                s => SessionActions.EnterCommandAction(s, Context.ConnectionId, action)
            );
        }
    }
}
