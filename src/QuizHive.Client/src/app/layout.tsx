import type { Metadata } from 'next'
import { Inter } from 'next/font/google'
import './globals.css'
import { ServerConnectionProvider } from '@/context/ServerConnectionContext'
import ConnectionBanner from '@/components/StatusBanner'

const inter = Inter({ subsets: ['latin'] })

const baseUrl = process.env.BASE_URL ?? "https://localhost:7195";

export const metadata: Metadata = {
  title: 'Quiz Hive',
  description: 'See https://github.com/jechtom/quiz-hive',
}

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <html lang="en">
      <body className={inter.className + ' bg-gradient-to-br from-cyan-50  to-gray-100'}>
        <ServerConnectionProvider baseUrl={baseUrl}>
          <ConnectionBanner />
          {children}
        </ServerConnectionProvider>
      </body>
    </html>
  )
}

