"use client";

import { SessionStateContext } from '@/context/SessionContext';
import { useContext } from 'react';
import JoinByCode from '@/components/JoinByCode';
import EnterName from '@/components/EnterName';
import Lobby from '@/components/Lobby';
import { ServerConnectionContext } from '@/context/ServerConnectionContext';
import Wait from '@/components/Wait';
import QuizAnswer from '@/components/QuizAnswer';

export default function Home() {
  const sessionContext = useContext(SessionStateContext);
  const serverConnection = useContext(ServerConnectionContext);

  
  if(!serverConnection.isConnectedToSession) {
    return (<JoinByCode />)
  } else if(sessionContext.currentScreen === 'EnterName') {
    return (<EnterName />)
  } else if(sessionContext.currentScreen === 'Lobby') {
    return (<Lobby />)
  } else if(sessionContext.currentScreen === 'Wait') {
    return (<Wait />)
  } else if(sessionContext.currentScreen === 'QuizAnswer') {
    return (<QuizAnswer />)
  } else {
    return (
      <main className="h-screen flex items-center justify-center">
        <div className="text-xl border p-5 border-red-500">Unknown screen: {sessionContext.currentScreen}</div>
      </main>
    );
  }
}
