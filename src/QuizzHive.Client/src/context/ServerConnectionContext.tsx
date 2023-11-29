"use client";

import React, { useContext, createContext, useState } from 'react'
import { ConnectionState, ConnectionStates, useSignalR } from './useSignalR'
import { CreateDefaultSessionState, SessionStateContext } from './SessionContext';
import { ISessionState } from './types';

export const ServerConnectionContext = createContext({
  state: ConnectionStates.Disconnected,
  proxy: {
    enterCode: (code: string) => {},
    enterName: (name: string) => {},
    enterAnswer: (code: string) => {}
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
  
  const signalR = useSignalR({
    url: baseUrl + "/hub",
    setConnectionState: setConnectionState,
    messageHandlers: [
      {
        message: 'PingMessage',
        handler: (data) => {
          //const limits = data?.[0] as undefined | LimitInfoExtended[]
          console.log(data);
        },
      },
      {
        message: 'UpdateStateMessage',
        handler: (data) => {
          //alert('new state!');
          var newGameState = data?.[data?.length - 1] as ISessionState;
          console.log(newGameState);
          setSessionState(newGameState);
        }
      }
    ],
  })

  const sendMessage = (method: string, message: any) =>
    async () => { await signalR.sendMessage(method, message); };

  const proxy = {
    enterCode(code: string) {
      signalR.sendMessage("EnterCodeMessage", code);
    },
    enterName(name: string) {
      signalR.sendMessage("EnterNameMessage", name);
    },
    enterAnswer(code: string) {
      signalR.sendMessage("EnterAnswerMessage", code);
    }
  };

  return (
    <ServerConnectionContext.Provider value={{ state: connectionState, proxy: proxy }}>
      <SessionStateContext.Provider value={sessionState}>
        {children}
      </SessionStateContext.Provider>
    </ServerConnectionContext.Provider>
  )
}
