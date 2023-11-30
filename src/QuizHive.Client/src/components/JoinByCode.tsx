"use client";

import { SessionStateContext } from '@/context/SessionContext';
import { ServerConnectionContext } from '@/context/ServerConnectionContext';
import Image from 'next/image'
import React, { useCallback, useContext, useState } from 'react';
import Header from './Header';
import Alert, { AlertType } from './Alert';

export default function JoinByCode() {
  const serverConnection = useContext(ServerConnectionContext);

  const [joinCode, setJoinCode] = useState('');

  const inputElement = useCallback((inputElement: HTMLElement | null) => {
    if (inputElement) {
      inputElement.focus();
    }
  }, []);

  function DisconnectedMessage() {
    if(serverConnection.disconnectedFromSession) {
        return (
            <div className="mb-10 relative">
                <Alert type={AlertType.Notify}>Session has ended.</Alert>
            </div>
        );
    }
    return (<></>);
  }

  function ErrorMessage() {
    if(serverConnection.connectToSessionFailed) {
        return (
            <div className="mt-3">
                <Alert type={AlertType.Error}>Failed to connect.</Alert>
            </div>
        );
    }
    return (<></>);
  }

  return (
    <main className="h-screen flex items-center justify-center">
      <form onSubmit={ async (e) => { e.preventDefault(); await serverConnection.proxy.enterCode(joinCode); } }>
        <div className="p-5">
          <DisconnectedMessage />
          <Header>
            Enter Code
          </Header>
          <div className="mt-3 relative">
            <input
              ref={inputElement}
              type="text"
              name="code"
              id="code"
              value={joinCode}
              onChange={e => setJoinCode(e.target.value)}
              className="block w-full shadow-lg rounded-md border-0 p-4 text-4xl text-gray-900 ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600"
            />
            <button
              type="submit"
              className="text-white bg-indigo-600 absolute end-2.5 bottom-2.5 px-4 py-3 hover:bg-blue-800 focus:ring-4 focus:outline-none focus:ring-blue-300 font-medium rounded-lg"
              >Join</button>
          </div>
          <ErrorMessage />
        </div>
      </form>
    </main>
  )
}
