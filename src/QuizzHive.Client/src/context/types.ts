export interface ISessionState {
    playersCount: number
    me: IMe
    currentScreen: string
    players: IPlayer[]
    segment: ISegment
}

export interface IMe {
    isNameSet: boolean
    name: string
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

export interface ISessionJoinResponse {
    success: boolean
    token: string
}