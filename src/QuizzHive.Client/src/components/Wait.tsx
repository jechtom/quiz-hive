"use client";

import HostControls from './HostControls';

export default function Lobby() {
  return (
    <main className="h-screen flex items-center justify-center">
      <div className="p-5">
        <div className="text-3xl m-5">
          Waiting for the host...
        </div>
      </div>
      <HostControls />
    </main>
  )
}
