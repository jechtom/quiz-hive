"use client";

import { SessionStateContext } from '@/context/SessionContext';
import { ServerConnectionContext } from '@/context/ServerConnectionContext';
import Image from 'next/image'
import React, { useCallback, useContext, useState } from 'react';
import { ServerApiContext } from '@/context/ServerApiContext';

export default function Home() {
  const serverConnection = useContext(ServerConnectionContext);
  const serverApi = useContext(ServerApiContext);

  const [joinCode, setJoinCode] = useState('');

  const inputElement = useCallback((inputElement: HTMLElement | null) => {
    if (inputElement) {
      inputElement.focus();
    }
  }, []);

  return (
    <>
    </>
  )
}
