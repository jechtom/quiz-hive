"use client";

export type HeaderProps = {
    children?: React.ReactNode
}

export default function Header(props: HeaderProps) {
  return (
    <h1 className="block font-extralight pb-5 text-5xl text-gray-900 text-center">{props.children}</h1>
  )
}
