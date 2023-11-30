"use client";

import { SessionStateContext } from '@/context/SessionContext';
import { useContext } from 'react';
import JoinByCode from '@/components/JoinByCode';
import EnterName from '@/components/EnterName';
import Lobby from '@/components/Lobby';
import { ServerConnectionContext, SessionConnectionState } from '@/context/ServerConnectionContext';
import QuizAnswer from '@/components/Quiz';
import Wait from '@/components/Wait';

export default function Home() {
  const sessionContext = useContext(SessionStateContext);
  const serverConnection = useContext(ServerConnectionContext);

  
  if(serverConnection.sessionConnectionState != SessionConnectionState.Connected) {
    return (<JoinByCode />)
  } else if(sessionContext.currentScreen === 'EnterName') {
    return (<EnterName />)
  } else if(sessionContext.currentScreen === 'Lobby') {
    return (<Lobby />)
  } else if(sessionContext.currentScreen === 'WaitForOthers') {
    return (<Wait>Wait for others. 😎</Wait>)
  } else if(sessionContext.currentScreen === 'WaitGetReady') {
    return (<Wait>Get ready! 😯</Wait>)
  } else if(sessionContext.currentScreen === 'Wait') {
    return (<Wait>Waiting for the host... ⏳</Wait>)
  } else if(sessionContext.currentScreen === 'End' && !sessionContext.me.isHost) {
    return (<Wait>Thanks for playing. 👋</Wait>)
  } else if(sessionContext.currentScreen === 'Quiz') {
    return (<QuizAnswer />)
  } else {
    return (
      <main className="h-screen flex items-center justify-center">
        <div className="text-xl border p-5 border-red-500">Unknown screen: {sessionContext.currentScreen}</div>
      </main>
    );
  }
}
