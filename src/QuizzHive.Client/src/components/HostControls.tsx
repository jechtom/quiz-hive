"use client";

import { ServerConnectionContext } from "@/context/ServerConnectionContext";
import { SessionStateContext } from "@/context/SessionContext";
import { useContext } from "react";


export default function HostControls() {
    const session = useContext(SessionStateContext);
    const serverConnection = useContext(ServerConnectionContext);

    if(session.me.isHost)
    {
        return (
            <div className="flex flex-row absolute end-2.5 bottom-2.5 gap-3">
                { session.me.hostControls.map((hc) => (
                    <button
                        key={ "host-action-" + hc.action } 
                        type="button" 
                        className="text-gray-800 p-2 hover:bg-blue-200 ring-2 ring-gray-300 font-medium rounded-lg"
                        onClick={ async () => await serverConnection.proxy.enterCommand(hc.action) }
                    >{hc.text}</button>
                ))}
            </div>
        );
    }
    else 
    {
        return (<></>);
    }
}