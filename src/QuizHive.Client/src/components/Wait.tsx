"use client";

import HostControls from './HostControls';

export type WaitProps = {
  children?: React.ReactNode
}

export default function Wait({ children } : WaitProps) {
  return (
    <main className="h-screen flex items-center justify-center">
      <div className="p-5">
        <div className="text-3xl m-5">
          {children}
        </div>
      </div>
      <HostControls />
    </main>
  )
}
