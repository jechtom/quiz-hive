"use client";

import { SessionStateContext } from '@/context/SessionContext';
import React, { useCallback, useContext, useState } from 'react';
import Header from './Header';
import { ServerConnectionContext } from '@/context/ServerConnectionContext';
import HostControls from './HostControls';
import Lead from './Lead';
import { IAnswerOption } from '@/context/types';

export default function QuizAnswer() {
  const session = useContext(SessionStateContext);
  const serverConnection = useContext(ServerConnectionContext);

  function ButtonLarge(props : { text: string, answer: IAnswerOption }) { return (
    <div className="w-full h-full p-2">
      <button type="button" className="w-full h-full text-white p-10 text-5xl bg-indigo-600 hover:bg-blue-800 focus:ring-4 focus:outline-none focus:ring-blue-300 font-medium rounded-lg"
        onClick={ async (e) => { e.preventDefault(); await serverConnection.proxy.setAnswer({ answers: [ props.answer.id ] }); } }>
        {props.text}
      </button>
    </div>
  )};

  function AnswerLarge(props : { text: string, answer: IAnswerOption }) { 
    
    if(session.quizSegment.showAnswers)
    {
      return (
        <div className="w-full h-full p-5">
          <div className={"grid gap-5 place-items-center w-full h-full p-10 text-5xl font-medium rounded-lg text-white " + (props.answer.isCorrect ? "bg-green-600" : "bg-red-800")}>
            <span>{props.text}</span>
            <span className="text-3xl">A A A </span>
          </div>
        </div>);
    }

    return (
    <div className="w-full h-full p-5">
      <div className="grid place-items-center w-full h-full p-10 text-5xl ring-4 ring-blue-300 font-medium rounded-lg">
        {props.text}
      </div>
    </div>);
};

  const LeadContent = () => {
    if(session.quizSegment.showAnswers) {
      return <></>;
    }
    return <>Waiting for {session.quizSegment.remainToAnswer} {(session.quizSegment.remainToAnswer == 1) ? "answer" : "answers"}</>
  };

  if(session.me.isHost) {
    return (
      <main className="h-screen flex items-center justify-center flex-col">
        <Header>{session.quizSegment.questionText}</Header>
        <Lead><LeadContent /></Lead>
        <div className="flex flex-col pt-10 w-full h-1/2">
          <div className="flex-1 flex">
            <div className="flex-1"><AnswerLarge text={session.quizSegment.answers[0].text} answer={session.quizSegment.answers[0]} /></div>
            <div className="flex-1"><AnswerLarge text={session.quizSegment.answers[1].text} answer={session.quizSegment.answers[1]} /></div>
          </div>
          <div className="flex-1 flex">
            <div className="flex-1"><AnswerLarge text={session.quizSegment.answers[2].text} answer={session.quizSegment.answers[2]} /></div>
            <div className="flex-1"><AnswerLarge text={session.quizSegment.answers[3].text} answer={session.quizSegment.answers[3]} /></div>
          </div>
        </div>
        <HostControls />
      </main>
    )
  }
  else
  {
    return (
      <main className="h-screen flex items-center justify-center">
        <div className="flex flex-col pt-10 w-full h-1/2">
          <div className="flex-1 flex">
            <div className="flex-1"><ButtonLarge text={session.quizSegment.answers[0].text} answer={session.quizSegment.answers[0]} /></div>
            <div className="flex-1"><ButtonLarge text={session.quizSegment.answers[1].text} answer={session.quizSegment.answers[1]} /></div>
          </div>
          <div className="flex-1 flex">
            <div className="flex-1"><ButtonLarge text={session.quizSegment.answers[2].text} answer={session.quizSegment.answers[2]} /></div>
            <div className="flex-1"><ButtonLarge text={session.quizSegment.answers[3].text} answer={session.quizSegment.answers[3]} /></div>
          </div>
        </div>
        <HostControls />
      </main>
    )
  }
}
