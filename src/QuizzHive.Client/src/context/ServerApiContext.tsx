"use client";

import React, { useContext, createContext, useState } from 'react'
import { ISessionJoinResponse, ISessionState } from './types';

export const ServerApiContext = createContext({
  joinByCode: (code: string) => {},
  apiCallInProgress: false
})

export type ServerApiProviderProps = {
  children?: React.ReactNode
  baseUrl: string
}

export const ServerApiProvider: React.FC<ServerApiProviderProps> = ({
    children,
    baseUrl
}) => {

  const [apiCallInProgress, setApiCallInProgress ] = useState(false);

  const joinByCode = async (code: string) => {
    setApiCallInProgress(true);
    await fetch(baseUrl + '/api/join-code', {
        method: 'POST',
        body: JSON.stringify({ code: code }),
        headers: { 'Content-type': 'application/json; charset=UTF-8' },
      })
      .then((response) => response.json())
      .then((data) => {
        var dataTyped = data as ISessionJoinResponse;
        if(!dataTyped.success) {
          
        }
      })
      .catch((err) => {
        console.log(err.message);
      })
      .finally(() => {
        setApiCallInProgress(false);
      });
  };

  return (
    <ServerApiContext.Provider value={{ 
      joinByCode: joinByCode, 
      apiCallInProgress: apiCallInProgress 
    }}>
        {children}
    </ServerApiContext.Provider>
  )
}
