"use client";

import { createContext } from 'react'
import { ISessionState } from './types';

export function CreateDefaultSessionState() : ISessionState {
    return {
        playersCount: 0,
        me: {
            isNameSet: false,
            name: ''
        },
        currentScreen: '',
        players: [],
        segment: {
            id: '',
            type: '',
            hasStarted: false,
            hasEnded: false
        }
    };
}

export const SessionStateContext = createContext<ISessionState>(CreateDefaultSessionState())