"use client";

export type LeadProps = {
    children?: React.ReactNode
}

export default function Lead(props: LeadProps) {
  return (
    <h1 className="text-3xl m-7">{props.children}</h1>
  )
}
