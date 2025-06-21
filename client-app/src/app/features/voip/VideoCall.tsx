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
    const prevHasVideo = useRef<boolean | undefined>(undefined);
    const [statefulCallClient, setStatefulCallClient] = useState<StatefulCallClient>();
    const callAgent = useRef<CallAgent>();
    const [call, setCall] = useState<Call>();
    const [playersCloseBox, setPlayersCloseBox] = useState<boolean>();
    const [playersFullScreen, setPlayersFullScreen] = useState<boolean>();

    useEffect(() => {
        mainStore.setWithoutSlider(true);
    }, [])
    useEffect(() => {
        const hasVideo = communicationStore.turnModel?.data.hasVideo;
        if (!prevHasVideo.current && hasVideo) {
            const initializeCall = async () => {
                if (!statefulCallClient) {
                    debugger;

                    const extraPlayerInfo = communicationStore.turnModel?.extraInfo.extraPlayerInfo || [];

                    const acsTokenObj = extraPlayerInfo.find(info => info.key === "acsToken");
                    // const acsTokenExpireObj = extraPlayerInfo.find(info => info.key === "acsTokenExpire");
                    const acsUserIdObj = extraPlayerInfo.find(info => info.key === "acsUserId");

                    const token = acsTokenObj?.value || "";
                    // const expiresOn = acsTokenExpireObj?.value || "";
                    const acsUserId = acsUserIdObj?.value || "";

                    // const isExpireDateValid = !isNaN(Date.parse(expiresOn));
                    // const isExpireDateInFuture = isExpireDateValid && new Date(expiresOn) > new Date();
                    // const isUserIdEmpty = !acsUserId || acsUserId.trim() === "";


                    // Create StatefulCallClient
                    const statefulCallClient = createStatefulCallClient({
                        userId: { communicationUserId: acsUserId },
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
        prevHasVideo.current = hasVideo;
    }, [communicationStore.turnModel?.data.hasVideo]);


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
        <div className="block-content">
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
    );
};

export default observer(VideoCall);

