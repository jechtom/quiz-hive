export interface ISessionState {
    playersCount: number
    me: IMe
    joinCode: string
    isUnlocked: boolean
    currentScreen: string
    players: IPlayer[]
    segment: ISegment
}

export interface IMe {
    isNameSet: boolean
    name: string
    isHost: boolean
    hostControls: IHostControl[]
}

export interface IPlayer {
    name: string
}

export interface ISegment {
    id: string
    hasStarted: boolean
    hasEnded: boolean
    type: string
}

export interface ISessionConnectedMessage {
    sessionId: string
    reconnectCode: string
}

export interface IAnswer {
    answers: string[]
}

export interface IHostControl {
    text: string
    action: string
}