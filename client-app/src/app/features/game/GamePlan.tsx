import { useEffect, useState } from "react";
import { CurrentRoomModel } from "../../models/CurrentRoomModel";
import { useStore } from "../../Store";
import agent from "../api/agent";
import GamePlayers from "./GamePlayes";
import { observer } from "mobx-react-lite";
import { useParams } from "react-router-dom";
import { AppConfig } from "../../models/AppConfig";
import VideoCall from "../voip/VideoCall";

function GamePlan() {

    const { roomId } = useParams<{ roomId: string }>();
    const { communicationStore, mainStore, profileStore } = useStore();
    const { initializeConnection, waitForConnection, disconnectConnection } = communicationStore;
    const [playersCloseBox, setPlayersCloseBox] = useState<boolean>();
    const [playersFullScreen, setPlayersFullScreen] = useState<boolean>();

    useEffect(() => {
        mainStore.setWithoutSlider(true);
    }, [])

    useEffect(() => {
        const setupConnection = async () => {
            let model: CurrentRoomModel;
            const jwtToken = agent.getToken() || "";
            await initializeConnection(jwtToken);
            await waitForConnection();
            if (!roomId) {
                console.error("Room id is undefined.");
                return;
            }
            else {
                model = { id: roomId };
            }
            try {
                debugger;
                const user = profileStore.logedInUSer();
                if (user?.userName) {
                    var isJoined = await communicationStore.addUserToRoom(
                        AppConfig.appId,
                        roomId,
                        user.userName
                    );
                    if (!isJoined) {
                        console.error("Failed to join room. Room may be full or does not exist.");
                        return;
                    }
                    console.log("User added to room");
                } else {
                    console.error("User info is incomplete.");
                }
            } catch (error) {
                console.error("Failed to add user to room:", error);
            }
        };

        setupConnection();

    }, [initializeConnection, waitForConnection, disconnectConnection]);

    const handlePlayersCloseBox = () => {
        setPlayersCloseBox(!playersCloseBox);
    }
    const handleFullScreen = () => {
        setPlayersFullScreen(!playersFullScreen);
    }

    console.log("alivePlayers", communicationStore.turnModel?.data?.alivePlayers);
    return (
        <main id="main-container">

            <div className="row g-0 flex-md-grow-1">
                <div className="col-md-3 col-lg-4 col-xl-2 bg-body-dark h100-scroll">
                    <div className="content">
                        <div className="d-lg push">
                            <div className={`block block-rounded ${playersCloseBox ? "block-mode-hidden" : ""} ${playersFullScreen ? "block-mode-fullscreen" : ""} `}>
                                <div className="block-header block-header-default block-header-rtl">
                                    <h3 className="block-title">Players</h3>
                                    <div className="block-options">
                                        <button type="button" className="btn-block-option" data-toggle="block-option" data-action="content_toggle" onClick={() => handlePlayersCloseBox()}><i className="si si-arrow-down"></i></button>
                                        <button type="button" className="btn-block-option" data-toggle="block-option" data-action="fullscreen_toggle" onClick={() => handleFullScreen()}><i className="si si-size-fullscreen"></i></button>
                                    </div>
                                </div>
                                <div className="block-content">
                                    {
                                        (communicationStore.turnModel?.data?.alivePlayers || []).map((player, index) => (
                                            <GamePlayers key={index} Player={player} Died={false} RoomId={roomId ?? ""} ActOnMe={false} />
                                        ))
                                    }
                                    {
                                        (communicationStore.turnModel?.data?.deadPlayers || []).map((player, index) => (
                                            <GamePlayers key={index} Player={player} Died={true} RoomId={roomId ?? ""} ActOnMe={false} />
                                        ))
                                    }
                                </div>
                            </div>
                        </div>
                        <div id="side-content" className="d-none d-lg-block push">
                            {
                                [...(communicationStore.turnModel?.data?.deadPlayers || []), ...(communicationStore.turnModel?.data?.deadPlayers || [])].map((player, index) => (
                                    <GamePlayers key={index} Player={player} Died={communicationStore.turnModel?.data?.deadPlayers.includes(player) ?? false} RoomId={roomId ?? ""} ActOnMe={false} />
                                ))
                            }
                        </div>
                    </div>
                </div>
                <div className="col-md-9 col-lg-8 col-xl-10">
                    <div className="content">
                        <div className="row g-sm">
                            {
                                (communicationStore.turnModel?.data?.actingOn || []).map((player, index) => (
                                    <div key={index} className="col-md-2 col-lg-2 col-xl-2">
                                        <GamePlayers Player={player} Died={false} RoomId={roomId ?? ""} ActOnMe={true} />
                                    </div>
                                ))
                            }
                        </div>
                    </div>
                    <VideoCall />
                </div>
            </div>
        </main>
    );
}

export default observer(GamePlan);