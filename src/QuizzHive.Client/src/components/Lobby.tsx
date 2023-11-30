"use client";

import { SessionStateContext } from '@/context/SessionContext';
import React, { useCallback, useContext, useState } from 'react';
import { IPlayer } from '@/context/types';
import HostControls from './HostControls';
import Header from './Header';
import Lead from './Lead';

export default function Lobby() {
  const sessionContext = useContext(SessionStateContext);

  const PlayersList = () => {
    const listItems = sessionContext.players.map((p,i) =>
      (<li key={"player-list-" + i} className="border-2 bg-white p-2 text-lg border-blue-400 rounded-md shadow-lg font-sans inline-block">{p.name}</li>)
    );
    return (
      <ul className="ml-auto flex gap-3 items-center flex-wrap">{listItems}</ul>
    );
  }

  const HostHeader = () => {
    const inner = sessionContext.isUnlocked 
      ? (<>Join with code <span className="shadow-lg rounded-md border-0 text-4xl text-gray-900 ring-1 ring-inset ring-gray-300 p-2 m-2">{sessionContext.joinCode}</span></>)
      : (<>Session is locked.</>);

    return <Lead>{inner}</Lead>
  }

  return (
    <main className="h-screen flex items-center justify-center">
      <div className="p-5">
        <Header>Lobby - Welcome!</Header>
        <HostHeader />
        <div className="m-3">
          <PlayersList />
        </div>
        <p className="block text-gray-800 text-center mt-3">Waiting for host to start. Players: {sessionContext.playersCount}</p>
      </div>
      <HostControls />
    </main>
  )
}
