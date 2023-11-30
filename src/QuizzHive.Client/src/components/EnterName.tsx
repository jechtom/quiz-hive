"use client";

import { SessionStateContext } from '@/context/SessionContext';
import { ServerConnectionContext } from '@/context/ServerConnectionContext';
import Image from 'next/image'
import React, { useCallback, useContext, useState } from 'react';
import Header from './Header';

export default function EnterName() {
  const serverConnection = useContext(ServerConnectionContext);

  const [name, setName] = useState('');

  const inputElement = useCallback((inputElement: HTMLElement | null) => {
    if (inputElement) {
      inputElement.focus();
    }
  }, []);

  return (
    <main className="h-screen flex items-center justify-center">
      <form onSubmit={ async (e) => { e.preventDefault(); await serverConnection.proxy.enterName(name); } }>
        <div className="p-5">
          <Header>
            Enter Your Name
          </Header>
          <div className="mt-3 relative">
            <input
              ref={inputElement}
              type="text"
              name="code"
              id="code"
              value={name}
              onChange={e => setName(e.target.value)}
              className="block w-full shadow-lg rounded-md border-0 p-4 text-3xl text-gray-900 ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600"
            />
            <button
              type="submit"
              className="text-white bg-indigo-600 absolute end-2.5 bottom-2.5 px-4 py-3 hover:bg-blue-800 focus:ring-4 focus:outline-none focus:ring-blue-300 font-medium rounded-lg"
              >Set</button>
          </div>
        </div>
      </form>
    </main>
  )
}
