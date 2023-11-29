import { useEffect, useState, useCallback, Dispatch, SetStateAction, useRef } from 'react'
import * as signalR from '@microsoft/signalr'

type Props = {
    url: string,
    setConnectionState: Dispatch<SetStateAction<ConnectionState>>,
    messageHandlers: Array<{
      message: string
      handler: (data: unknown[]) => void
    }>
}

export type ConnectionState = {
    isConnected: boolean,
    isConnecting: boolean,
    text: string
}

export namespace ConnectionStates {
    export const Connected: ConnectionState = { isConnected: true, isConnecting: false, text: "Connected" }
    export const Disconnected: ConnectionState = { isConnected: false, isConnecting: false, text: "Disconnected" }
    export const Connecting: ConnectionState = { isConnected: false, isConnecting: true, text: "Connecting..." }
};

export const createSignalRConnection = (url: string) => 
    new signalR.HubConnectionBuilder()
        .withUrl(url)
        .configureLogging(signalR.LogLevel.Information)
        .withAutomaticReconnect({
          nextRetryDelayInMilliseconds: (retryContext) => {
            // incrementally increase retry delays
            if (retryContext.elapsedMilliseconds > 10000) {
              // after 10s, retry every 2s
              return 2000;
            } else if (retryContext.elapsedMilliseconds > 5000) {
              // after 5s, retry every 1s
              return 1000;
            } else {
              // try every 500ms
              return 500;
            }
          },
        })
        .build();

export interface ISignalRSender {
  sendMessage(method: string, message: any) : Promise<void>
}

export const useSignalR = ({
    url,
    setConnectionState,
    messageHandlers
  }: Props) : ISignalRSender => {
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null)
    const connectionInitialized = useRef(false);

    const startSignalRConnection = useCallback(async (url: string) => {
      if(connectionInitialized.current) {
        return;
      }

      const connection = createSignalRConnection(url);
      setConnection(connection);
      
      connectionInitialized.current = true;

      const start = async () => {
        try {
          if (connection.state !== 'Connected') {
            setConnectionState(ConnectionStates.Connecting)
            await connection.start().then(() => setConnectionState(ConnectionStates.Connected));
          }
        } catch (e) {
          setTimeout(() => {
            start()
          }, 1000)
        }
      }
  
      connection.on('send', (data) => {
        console.log('send', data)
      })

      connection.onreconnecting((error) => {
        console.log('onreconnecting', error)
        setConnectionState(ConnectionStates.Connecting)
      })

      connection.onreconnected((connectionId) => {
        console.log('onreconnected', connectionId)
        setConnectionState(ConnectionStates.Connected)
      })
      
      connection.onclose((error) => {
        console.log('onclose', error)
        setConnectionState(ConnectionStates.Disconnected)
        start()
      })
  
      // add message listeners
      messageHandlers.forEach(({ message, handler }) =>
        // react to messages from backend
        connection?.on(message, (...data) => {
          try {
            handler(data)
          } catch (error) {
            console.log('handle message error', error)
          }
        })
      )
  
      start()
    }, [])
  
    useEffect(() => {
      console.log('Starting SignalR connection')
      startSignalRConnection(url);
    }, [startSignalRConnection, url])

    return {
      sendMessage(methodName: string, message: any) : Promise<void> {
        return connection?.send(methodName, message) ?? Promise.reject("Connection not yet initialized.");
      }
    }
  }
  