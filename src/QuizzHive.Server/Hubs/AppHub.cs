using Microsoft.AspNetCore.SignalR;
using QuizzHive.Server.DataLayer;
using QuizzHive.Server.Services;
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

        public async Task EnterCodeMessage(string code)
        {
            logger.LogDebug("Code from client: {Name}", code);

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

            try
            {
                // try add player to the session
                bool result = await sessionActionDispatcher.TryDispatchActionAsync(
                    sessionId,
                    s => SessionActions.TryConnectWithCode(s, Context.ConnectionId, code),
                    async s =>
                    {
                        // add to session group
                        await Groups.AddToGroupAsync(Context.ConnectionId, s.SessionId);
                    }
                );

                if(!result)
                {
                    clientsManager.RemoveSessionLink(Context.ConnectionId, sessionId);
                }
            }
            catch
            {
                clientsManager.RemoveSessionLink(Context.ConnectionId, sessionId);
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
    }
}
