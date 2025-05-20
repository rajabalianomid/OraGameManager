import "./VideoCall.css"
import { useEffect, useRef, useState } from "react";
import { FluentThemeProvider, CallClientProvider, CallAgentProvider, CallProvider, StatefulCallClient, createStatefulCallClient, DEFAULT_COMPONENT_ICONS } from "@azure/communication-react";
import { initializeIcons, registerIcons } from "@fluentui/react";
import { Call, CallAgent } from "@azure/communication-calling";
import { AzureCommunicationTokenCredential } from "@azure/communication-common";
import { useStore } from "../../Store";
import CallingComponents from "./CallingComponents";
import { observer } from "mobx-react-lite";

initializeIcons();
registerIcons({ icons: DEFAULT_COMPONENT_ICONS });
const getRandomNumber = () => {
    return Math.floor(Math.random() * 1000);
};


function VideoCall() {
    const groupId = "6bf9f7d7-89f4-4ff2-8974-a3468f282a1c"; // Group ID for call
    const displayName = `user ${getRandomNumber()}`;

    const { communicationStore, mainStore } = useStore();
    const { onCall } = communicationStore;
    const [statefulCallClient, setStatefulCallClient] = useState<StatefulCallClient>();
    const callAgent = useRef<CallAgent>();
    const [call, setCall] = useState<Call>();
    const [playersCloseBox, setPlayersCloseBox] = useState<boolean>();
    const [playersFullScreen, setPlayersFullScreen] = useState<boolean>();

    useEffect(() => {
        mainStore.setWithoutSlider(true);
    }, [])
    useEffect(() => {
        if (onCall) {
            const initializeCall = async () => {
                if (!statefulCallClient) {
                    debugger;
                    await communicationStore.getUser();
                    const { userId, token } = communicationStore.videoCallToken;

                    // Create StatefulCallClient
                    const statefulCallClient = createStatefulCallClient({
                        userId: { communicationUserId: userId },
                    });

                    statefulCallClient.getDeviceManager().then((deviceManager) => {
                        deviceManager.askDevicePermission({ video: true, audio: true });
                    });


                    setStatefulCallClient(statefulCallClient);

                    const tokenCredential = new AzureCommunicationTokenCredential(token);

                    // Create CallAgent
                    callAgent.current = await statefulCallClient.createCallAgent(
                        tokenCredential,
                        {
                            displayName: displayName,
                        }
                    );
                }

                if (callAgent.current) {
                    setCall(
                        callAgent.current.join(
                            { groupId },
                            {
                                audioOptions: { muted: true },
                                videoOptions: { localVideoStreams: undefined },
                            }
                        )
                    );
                }
            };
            initializeCall();
        }
    }, [onCall]);


    const handlePlayersCloseBox = () => {
        setPlayersCloseBox(!playersCloseBox);
    }
    const handleFullScreen = () => {
        setPlayersFullScreen(!playersFullScreen);
    }
    const handleStart = () => {
        debugger;
        communicationStore.setOnCall(true);
    }

    return (
        <main id="main-container">
            <div className="row g-0 flex-md-grow-1">
                <div className="col-md-4 col-lg-5 col-xl-3 bg-body-dark h100-scroll">
                    <div className="content">
                        <div className="d-md-none push">
                            <div className={`block block-rounded ${playersCloseBox ? "block-mode-hidden" : ""} ${playersFullScreen ? "block-mode-fullscreen" : ""} `}>
                                <div className="block-header block-header-default block-header-rtl">
                                    <h3 className="block-title">Players</h3>
                                    <div className="block-options">
                                        <button type="button" className="btn-block-option" data-toggle="block-option" data-action="content_toggle" onClick={() => handlePlayersCloseBox()}><i className="si si-arrow-down"></i></button>
                                        <button type="button" className="btn-block-option" data-toggle="block-option" data-action="fullscreen_toggle" onClick={() => handleFullScreen()}><i className="si si-size-fullscreen"></i></button>
                                    </div>
                                </div>
                                <div className="block-content">
                                    {/* <GamePlayers /> */}
                                </div>
                            </div>
                        </div>
                        <div id="side-content" className="d-none d-lg-block push">
                            {/* <GamePlayers /> */}
                        </div>
                    </div>
                </div>
                <div className="col-md-8 col-lg-7 col-xl-9">
                    <div className="content content-full">
                        <div className="block block-rounded">
                            <div className="block-content">
                                <div className="mb-12">
                                    <button type="submit" className="btn btn-primary" onClick={() => handleStart()}>Start</button>
                                </div>
                                <FluentThemeProvider>
                                    {statefulCallClient && (
                                        <CallClientProvider callClient={statefulCallClient}>
                                            {callAgent && (
                                                <CallAgentProvider callAgent={callAgent.current}>
                                                    {call && (
                                                        <CallProvider call={call}>
                                                            <CallingComponents />
                                                        </CallProvider>
                                                    )}
                                                </CallAgentProvider>
                                            )}
                                        </CallClientProvider>
                                    )}
                                </FluentThemeProvider>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </main>
    );
};

export default observer(VideoCall);

