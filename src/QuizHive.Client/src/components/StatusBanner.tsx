"use client";

import { ServerConnectionContext } from "@/context/ServerConnectionContext";
import { useContext } from "react";


export default function ConnectionBanner() {
    const serverConnection = useContext(ServerConnectionContext);

    function StatusBannerContent() {
        if(serverConnection.state.isConnected) {
            return { content: (<>Connected.</>), bgclass: 'bg-green-700', show: true };
        } else if(serverConnection.state.isConnecting) {
            return { content: (<>Connecting...</>), bgclass: 'bg-orange-700', show: true };
        } else {
            return { content: (<></>), bgclass: '', show: false };
        }
    }

    var { content, bgclass, show } = StatusBannerContent();

    if(show)
    {
        return (
            <div id="banner" tabIndex={-1} className={"flex fixed z-50 justify-between items-start py-2 px-4 font-light w-full text-white sm:items-center shadow-lg " + bgclass }>
                <p className="text-white">{content}</p>
            </div>
        );
    }
    else 
    {
        return (<></>);
    }
}