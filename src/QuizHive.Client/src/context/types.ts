export interface ISessionState {
    playersCount: number
    me: IMe
    joinCode: string
    isUnlocked: boolean
    currentScreen: string
    players: IPlayer[]
    segment: ISegment
    quizSegment: IQuizSegment
}

export interface IQuizSegment {
    answers: IAnswerOption[]
    remainToAnswer: number
    questionText: string
    showAnswers: boolean
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
    hasEnded: boolean
    type: string
}

export interface IAnswerOption {
    text: string,
    id: string,
    isCorrect: boolean
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