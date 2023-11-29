using QuizzHive.Server.Hubs.Messages;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace QuizzHive.Server.Services
{
    /// <summary>
    /// Singleton thread-safe in-memory store of SignalR connections.
    /// </summary>
    public class ClientsManager
    {
        record ClientInfo()
        {
            public static ClientInfo Empty { get; } = new();
            public string? SessionId { get; init; } 
        }

        ConcurrentDictionary<string, ClientInfo> clients = new();

        public void OnClientConnected(string connectionId)
        {
            if (connectionId is null)
            {
                throw new ArgumentNullException(nameof(connectionId));
            }

            if (!clients.TryAdd(connectionId, ClientInfo.Empty))
            {
                throw new InvalidOperationException("Client with given Id is already connected.");
            }
        }

        public void OnClientDisconnected(string connectionId, out string? linkedSessionId)
        {
            if (connectionId is null)
            {
                throw new ArgumentNullException(nameof(connectionId));
            }

            if (!clients.TryRemove(connectionId, out ClientInfo? clientInfo))
            {
                linkedSessionId = default;
                return;
            }

            linkedSessionId = clientInfo.SessionId;
        }

        public bool TryGetSessionLink(string connectionId, [NotNullWhen(true)]out string? linkedSessionId)
        {
            if (!clients.TryGetValue(connectionId, out ClientInfo? clientInfo) || clientInfo.SessionId == null)
            {
                // client not found or no session linked
                linkedSessionId = default;
                return false;
            }

            linkedSessionId = clientInfo.SessionId;
            return true;
        }

        public bool TryAssignSessionLink(string connectionId, string sessionId)
        {
            if (string.IsNullOrEmpty(connectionId))
            {
                throw new ArgumentException($"'{nameof(connectionId)}' cannot be null or empty.", nameof(connectionId));
            }

            if (string.IsNullOrEmpty(sessionId))
            {
                throw new ArgumentException($"'{nameof(sessionId)}' cannot be null or empty.", nameof(sessionId));
            }

            while (true)
            {
                if (!clients.TryGetValue(connectionId, out ClientInfo? clientInfo))
                {
                    // client not found - probably has just disconnected
                    return false;
                }

                if (clientInfo.SessionId != null)
                {
                    // client already assigned
                    return false;
                }

                // update or return false if value has been changed in between
                var success = clients.TryUpdate(connectionId,
                    clientInfo with
                    {
                        SessionId = sessionId
                    }, clientInfo
                );

                if (success)
                {
                    return true;
                }
            }
        }

        public void RemoveSessionLink(string connectionId, string sessionId)
        {
            while (true)
            {
                if (!clients.TryGetValue(connectionId, out ClientInfo? clientInfo))
                {
                    // client not found - probably has just disconnected
                    return;
                }

                if(clientInfo.SessionId != sessionId)
                {
                    // different session assigned
                    return;
                }

                // update or return false if value has been changed in between
                bool success = clients.TryUpdate(connectionId,
                    clientInfo with
                    {
                        SessionId = null
                    }, clientInfo
                );

                if(success)
                {
                    return;
                }
            }
        }
    }
}
