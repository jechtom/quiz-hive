import type { Metadata } from 'next'
import { Inter } from 'next/font/google'
import './globals.css'
import SignalRProvider, { SignalRProviderProps } from '../context/SignalRContext'

const inter = Inter({ subsets: ['latin'] })

const baseUrl = process.env.BASE_URL ?? "https://localhost:7195";

export const metadata: Metadata = {
  title: 'Quizz Hive',
  description: 'See https://github.com/jechtom/quizz-hive',
}

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <html lang="en">
      <body className={inter.className}>
        <SignalRProvider baseUrl={baseUrl}>
          {children}
        </SignalRProvider>
      </body>
    </html>
  )
}

