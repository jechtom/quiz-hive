"use client";

import React, { useContext, createContext, useState, useEffect } from 'react'
import { ConnectionState, ConnectionStates, useSignalR } from './useSignalR'
import { CreateDefaultSessionState, SessionStateContext } from './SessionContext';
import { IAnswer, ISessionConnectedMessage, ISessionState } from './types';

export enum SessionConnectionState { 
  AppStarted, // not yet connected to or disconnected from any session
  InvalidJoinCode, // invalid join code
  Connected, // connected to the session
  SessionEnded, // the session has ended, no need to reconnect
  Kicked, // you have been removed from the session, no need to reconnect
  LostConnection, // connection to the server has been lost, we should try to reconnect
  Left // connection left on purpose by client
}

export const ServerConnectionContext = createContext({
  state: ConnectionStates.Disconnected,
  sessionConnectionState: SessionConnectionState.AppStarted,
  proxy: {
    sessionJoinWithCode: (code: string) => {},
    setName: (name: string) => {},
    setAnswer: (answer: IAnswer) => {},
    hostCommand: (command: string) => {},
    sessionLeave: () => {}
  }
})

export type ServerConnectionProviderProps = {
  children?: React.ReactNode
  baseUrl: string
}

export const ServerConnectionProvider: React.FC<ServerConnectionProviderProps> = ({
    children,
    baseUrl
}) => {
  const [connectionState, setConnectionState] = useState<ConnectionState>(ConnectionStates.Disconnected)

  const [sessionState, setSessionState] = useState<ISessionState>(CreateDefaultSessionState)

  const [sessionConnectionState, setSessionConnectionState] = useState(SessionConnectionState.AppStarted)

  const [reconnectCode, setReconnectCode] = useState<null | ISessionConnectedMessage>(null)

  const localStorageReconnectKey = "reconnect-key";

  const signalR = useSignalR({
    url: baseUrl + "/hub",
    setConnectionState: setConnectionState,
    messageHandlers: [
      {
        message: 'Update',
        handler: (data) => {
          var newGameState = data?.[data?.length - 1] as ISessionState;
          console.log(newGameState);
          setSessionState(newGameState);
        }
      },
      {
        message: 'InvalidJoinCode',
        handler: (data) => {
          setSessionConnectionState(SessionConnectionState.InvalidJoinCode);
        }
      },
      {
        message: 'SessionConnect',
        handler: (data) => {
          var connectedMessage = data?.[data?.length - 1] as ISessionConnectedMessage;
          setSessionConnectionState(SessionConnectionState.Connected);
          setReconnectCode(connectedMessage);
        }
      },
      {
        message: 'SessionEnd',
        handler: (data) => {
          setSessionConnectionState(SessionConnectionState.SessionEnded);
        }
      },
      {
        message: 'SessionKick',
        handler: (data) => {
          setSessionConnectionState(SessionConnectionState.Kicked);
        }
      },
      {
        message: 'SessionLeave',
        handler: (data) => {
          reconnectCodeReset(); // reconnect code no longer needed
          setSessionConnectionState(SessionConnectionState.Left);
        }
      }
    ],
  })

  const sendMessage = async (method: string, message: any) : Promise<void> =>
  {
    try {
      await signalR.sendMessage(method, message);
    } catch {
    }
  };

  const proxy = {
    sessionJoinWithCode(code: string) {
      sendMessage("SessionJoinWithCode", code);
    },
    setName(name: string) {
      sendMessage("SetName", name);
    },
    setAnswer(answer: IAnswer) {
      sendMessage("SetAnswer", answer);
    },
    hostCommand(command: string) {
      sendMessage("HostCommand", command);
    },
    sessionReconnect(sessionId: string, reconnectCode: string) {
      sendMessage("SessionReconnect", { sessionId, reconnectCode });
    },
    sessionLeave() {
      sendMessage("SessionLeave", {});
    }
  };
  
  // handle signalR disconnect without any reason
  useEffect(() => {
    if(!connectionState.isConnected && sessionConnectionState == SessionConnectionState.Connected)
    {
      // disconnected without reason
      setSessionConnectionState(SessionConnectionState.LostConnection);
    }
  }, [ connectionState, sessionConnectionState ])

  // handle reconnect session
  useEffect(() => {
    // can't reconnect session if not connected to signalR hub
    if(!connectionState.isConnected) return;

    var data : ISessionConnectedMessage | null = null;

    // reconnect on unexpected server disconnected
    if(sessionConnectionState == SessionConnectionState.LostConnection)
    {
      // try reconnect to last connected session (kept in memory)
      // - this is for temporal connection loss, server restart, etc.
      data = reconnectCode;
    } 
    // reconnect on page refresh
    else if(sessionConnectionState == SessionConnectionState.AppStarted)
    {
      // then try to reconnect to last connected session (recover from localstorage)
      // - this is for accidental page refresh
      data = reconnectCodeGetFromStorage();
    }

    if(data !== null)
    {
      console.log("Reconnect to session attempt.", data);
      proxy.sessionReconnect(data.sessionId, data.reconnectCode);
    }
  }, [connectionState, sessionConnectionState, reconnectCode])

  // store last connect data to local storage to survive accidental page refresh
  useEffect(() => {
    if(reconnectCode !== null)
    {
      localStorage.setItem(localStorageReconnectKey, JSON.stringify(reconnectCode));
    }
  }, [reconnectCode, connectionState])

  const reconnectCodeReset = () => {
    setReconnectCode(null);
    localStorage.removeItem(localStorageReconnectKey);
  }

  const reconnectCodeGetFromStorage = () => {
    var json = localStorage.getItem(localStorageReconnectKey);
    return (json === null) ? null : JSON.parse(json) as ISessionConnectedMessage | null;
  }

  return (
    <ServerConnectionContext.Provider value={{ 
      state: connectionState, 
      sessionConnectionState: sessionConnectionState,
      proxy: proxy
    }}>
      <SessionStateContext.Provider value={sessionState}>
        {children}
      </SessionStateContext.Provider>
    </ServerConnectionContext.Provider>
  )
}
