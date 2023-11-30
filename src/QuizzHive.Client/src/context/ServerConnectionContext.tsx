"use client";

import React, { useContext, createContext, useState, useEffect } from 'react'
import { ConnectionState, ConnectionStates, useSignalR } from './useSignalR'
import { CreateDefaultSessionState, SessionStateContext } from './SessionContext';
import { IAnswer, ISessionConnectedMessage, ISessionState } from './types';

export const ServerConnectionContext = createContext({
  state: ConnectionStates.Disconnected,
  isConnectedToSession: false,
  connectToSessionFailed: false,
  disconnectedFromSession: false,
  proxy: {
    enterCode: (code: string) => {},
    enterName: (name: string) => {},
    enterAnswer: (answer: IAnswer) => {},
    enterCommand: (command: string) => {}
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

  const [isConnectedToSession, setIsConnectedToSession] = useState(false)
  const [connectToSessionFailed, setConnectToSessionFailed] = useState(false)
  const [disconnectedFromSession, setDisconnectedFromSession] = useState(false)

  const [lastConnect, setLastConnect] = useState<null | ISessionConnectedMessage>(null)

  const signalR = useSignalR({
    url: baseUrl + "/hub",
    setConnectionState: setConnectionState,
    messageHandlers: [
      {
        message: 'UpdateStateMessage',
        handler: (data) => {
          var newGameState = data?.[data?.length - 1] as ISessionState;
          console.log(newGameState);
          setSessionState(newGameState);
        }
      },
      {
        message: 'WrongJoinCodeMessage',
        handler: (data) => {
          setConnectToSessionFailed(true);
        }
      },
      {
        message: 'SessionConnectedMessage',
        handler: (data) => {
          var connectedMessage = data?.[data?.length - 1] as ISessionConnectedMessage;
          setConnectToSessionFailed(false);
          setIsConnectedToSession(true);
          setLastConnect(connectedMessage);
        }
      },
      {
        message: 'SessionDisconnectedMessage',
        handler: (data) => {
          setIsConnectedToSession(false);
          setDisconnectedFromSession(true);
        }
      }
    ],
  })

  const proxy = {
    enterCode(code: string) {
      setConnectToSessionFailed(false);
      signalR.sendMessage("EnterCodeMessage", code);
    },
    enterName(name: string) {
      signalR.sendMessage("EnterNameMessage", name);
    },
    enterAnswer(answer: IAnswer) {
      signalR.sendMessage("EnterAnswerMessage", answer);
    },
    enterCommand(command: string) {
      signalR.sendMessage("EnterCommandMessage", command);
    },
    enterReconnectMessage(sessionId: string, reconnectCode: string) {
      signalR.sendMessage("EnterReconnectMessage", { sessionId, reconnectCode });
    }
  };
    
  useEffect(() => {
    if(!connectionState.isConnected)
    {
      if(isConnectedToSession)
      {
        // if previously connected, show it has been disconnected
        setDisconnectedFromSession(true);
      }
      setIsConnectedToSession(false);
    }
  }, [connectionState, isConnectedToSession])

  useEffect(() => {
    if(connectionState.isConnected && !isConnectedToSession)
    {
      // try reconnect to last connected session (kept in memory)
      // - this is for temporal connection loss, server restart, etc.
      var data : ISessionConnectedMessage | null = lastConnect;

      if(data === null)
      {
        // then try to reconnect to last connected session (recover from localstorage)
        // - this is for accidental page refresh
        var json = localStorage.getItem('lastConnect')
        data = (json === null) ? null : JSON.parse(json) as ISessionConnectedMessage | null;
      }

      if(data !== null)
      {
        console.log("Reconnect to session attempt.", data);
        proxy.enterReconnectMessage(data.sessionId, data.reconnectCode);
      }
    }
  }, [connectionState, lastConnect, isConnectedToSession])

  useEffect(() => {
    if(lastConnect !== null)
    {
      // store last connect data to local storage to survive accidental page refresh
      localStorage.setItem('lastConnect', JSON.stringify(lastConnect));
    }
  }, [lastConnect])

  return (
    <ServerConnectionContext.Provider value={{ 
      state: connectionState, 
      proxy: proxy,
      isConnectedToSession: isConnectedToSession,
      connectToSessionFailed: connectToSessionFailed,
      disconnectedFromSession: disconnectedFromSession
    }}>
      <SessionStateContext.Provider value={sessionState}>
        {children}
      </SessionStateContext.Provider>
    </ServerConnectionContext.Provider>
  )
}
