"use client";

import { SessionStateContext } from '@/context/SessionContext';
import React, { useCallback, useContext, useState } from 'react';
import Header from './Header';

export default function QuizAnswer() {
  const session = useContext(SessionStateContext);

  function ButtonLarge(props : { text: string }) { return (
    <div className="w-full h-full p-2">
      <button type="button" className="w-full h-full text-white p-10 text-5xl bg-indigo-600 hover:bg-blue-800 focus:ring-4 focus:outline-none focus:ring-blue-300 font-medium rounded-lg" >
        {props.text}
      </button>
    </div>
  )};

  return (
    <main className="h-screen flex items-center justify-center">
      <div className="flex flex-col pt-10 w-full h-1/2">
        <div className="flex-1 flex">
          <div className="flex-1"><ButtonLarge text="A" /></div>
          <div className="flex-1"><ButtonLarge text="B" /></div>
        </div>
        <div className="flex-1 flex">
          <div className="flex-1"><ButtonLarge text="C" /></div>
          <div className="flex-1"><ButtonLarge text="D" /></div>
        </div>
      </div>
    </main>
  )
}
