"use client";

export type AlertProps = {
    children?: React.ReactNode,
    type: AlertType
}

export enum AlertType {
  Notify,
  Error
}

export default function Alert(props: AlertProps) {
  switch(props.type)
  {
    case AlertType.Error:
      return (
        <div className="block w-full shadow-lg rounded-md border-0 p-4 text-white ring-1 ring-inset bg-orange-700 ring-gray-300">
          {props.children}
        </div>);
    case AlertType.Notify:
      return (
        <div className="block w-full shadow-lg rounded-md border-0 p-4 text-black ring-1 ring-inset bg-amber-200 ring-gray-300">
          {props.children}
        </div>);
    default:
      return (<>{props.children}</>);
  }
}
