"use client";

import React, { useContext, createContext, useState } from 'react'
import { ConnectionState, ConnectionStates, useSignalR } from './useSignalR'

export const SignalRContext = createContext({
  state: ConnectionStates.Disconnected,
  proxy: {
    enterCode: (code: string) => {},
    enterName: (name: string) => {},
    enterAnswer: (code: string) => {}
  },
  gameState: {}
})

export type SignalRProviderProps = {
  children?: React.ReactNode
  baseUrl: string
}

const SignalRProvider: React.FC<SignalRProviderProps> = ({
    children,
    baseUrl
}) => {
  const [connectionState, setConnectionState] = useState<ConnectionState>(ConnectionStates.Disconnected)
  const [gameState, setGameState] = useState<any>(null)

  
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
          var newGameState = data?.[data?.length - 1];
          console.log(newGameState);
          setGameState(newGameState);
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
    <SignalRContext.Provider value={{ state: connectionState, proxy: proxy, gameState: gameState }}>{children}</SignalRContext.Provider>
  )
}

export default SignalRProvider
