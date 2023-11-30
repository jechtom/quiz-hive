"use client";

import { SessionStateContext } from '@/context/SessionContext';
import { ServerConnectionContext, SessionConnectionState } from '@/context/ServerConnectionContext';
import Image from 'next/image'
import React, { useCallback, useContext, useState } from 'react';
import Header from './Header';
import Alert, { AlertType } from './Alert';
import { Chilanka } from 'next/font/google';

export default function JoinByCode() {
  const serverConnection = useContext(ServerConnectionContext);

  const [joinCode, setJoinCode] = useState('');

  const inputElement = useCallback((inputElement: HTMLElement | null) => {
    if (inputElement) {
      inputElement.focus();
    }
  }, []);

  function StateMessage() {
    
    const Wrapper = (children?: React.ReactNode) =>
      (<div className="mb-10 relative">{children}</div>);

    switch(serverConnection.sessionConnectionState)
    {
      case SessionConnectionState.LostConnection:
        return Wrapper(<Alert type={AlertType.Error}>Connection to the server has been lost.</Alert>);
      case SessionConnectionState.InvalidJoinCode:
        return Wrapper(<Alert type={AlertType.Error}>Invalid join code.</Alert>);
      case SessionConnectionState.Kicked:
        return Wrapper(<Alert type={AlertType.Error}>You have been removed from the session.</Alert>);
      case SessionConnectionState.SessionEnded:
        return Wrapper(<Alert type={AlertType.Notify}>Sessino has ended.</Alert>);
      default:
        return null;
    }
  }

  return (
    <main className="h-screen flex items-center justify-center">
      <form onSubmit={ async (e) => { e.preventDefault(); await serverConnection.proxy.sessionJoinWithCode(joinCode); } } method="post">
        <div className="p-5">
          <StateMessage />
          <Header>
            Enter Code
          </Header>
          <div className="mt-3 relative">
            <input
              ref={inputElement}
              type="text"
              name="code"
              id="code"
              autoComplete="off"
              value={joinCode}
              required
              onChange={e => setJoinCode(e.target.value)}
              className="block w-full shadow-lg rounded-md border-0 p-4 text-4xl text-gray-900 ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600"
            />
            <button
              type="submit"
              className="text-white bg-indigo-600 absolute end-2.5 bottom-2.5 px-4 py-3 hover:bg-blue-800 focus:ring-4 focus:outline-none focus:ring-blue-300 font-medium rounded-lg"
              >Join</button>
          </div>
        </div>
      </form>
    </main>
  )
}
