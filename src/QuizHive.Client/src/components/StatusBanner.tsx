"use client";

import { ServerConnectionContext, SessionConnectionState } from "@/context/ServerConnectionContext";
import { SessionStateContext } from "@/context/SessionContext";
import { useContext } from "react";


export default function ConnectionBanner() {
    const serverConnection = useContext(ServerConnectionContext);
    const sessionContext = useContext(SessionStateContext);

    function StatusBannerContent() {
        if(serverConnection.state.isConnected) {
            return { content: (<>Connected.</>), bgclass: 'bg-green-700', show: true };
        } else if(serverConnection.state.isConnecting) {
            return { content: (<>Connecting...</>), bgclass: 'bg-orange-700', show: true };
        } else {
            return { content: (<></>), bgclass: '', show: false };
        }
    }

    function ConnectionCodeContent() {
        if(serverConnection.sessionConnectionState != SessionConnectionState.Connected || !sessionContext.me.isHost) {
            return <></>;
        }

        if(sessionContext.isUnlocked)
        {
            return (<span> Join code is <span className="font-bold ring-1 ring-inset ring-gray-300 p-1">{sessionContext.joinCode}</span>.</span>);
        }
        else
        {
            return (<span> Session is locked.</span>);

        }
    }

    function DisconnectContent() {
        if(serverConnection.sessionConnectionState != SessionConnectionState.Connected) {
            return <></>;
        }
        return (<p className="float-right"><button onClick={ async (e) => { e.preventDefault(); await serverConnection.proxy.sessionLeave(); }} className="text-white hover:bg-blue-200 ring-2 pl-1 pr-1 ml-4 ring-gray-300 font-medium rounded-lg">Leave session</button></p>);
    }

    var { content, bgclass, show } = StatusBannerContent();
    var connectionCodeContent = ConnectionCodeContent();
    var disconnectContent = DisconnectContent();

    if(show)
    {
        return (
            <div id="banner" tabIndex={-1} className={"flex fixed z-50 justify-between items-start py-2 px-4 font-light w-full text-white sm:items-center shadow-lg " + bgclass }>
                <p className="text-white">{content}{connectionCodeContent}</p>
                {disconnectContent}
            </div>
        );
    }
    else 
    {
        return (<></>);
    }
}