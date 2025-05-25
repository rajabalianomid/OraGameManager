import { useEffect, useState } from "react";
import { CurrentRoomModel } from "../../models/CurrentRoomModel";
import { useStore } from "../../Store";
import agent from "../api/agent";
import GamePlayers from "./GamePlayes";
import { observer } from "mobx-react-lite";
import GameAction from "./GameAction";
import { useParams } from "react-router-dom";
import { AppConfig } from "../../models/AppConfig";

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
                model = { id: Number(roomId) };
            }
            try {
                debugger;
                const user = profileStore.logedInUSer();
                if (user?.userName) {
                    await communicationStore.addUserToRoom(
                        AppConfig.appId,
                        roomId,
                        user.userName
                    );
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


    return (
        <main id="main-container">
            <div className="row g-0 flex-md-grow-1">
                <div className="col-md-4 col-lg-5 col-xl-3 bg-body-dark h100-scroll">
                    <div className="content">
                        <div className="d-lg-none push">
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
                                        [...(communicationStore.turnModel?.players || []), ...(communicationStore.turnModel?.diedPlayers || [])].map((player, index) => (
                                            <GamePlayers key={index} Player={player} Died={communicationStore.turnModel?.diedPlayers.includes(player) ?? false} />
                                        ))
                                    }
                                </div>
                            </div>
                        </div>
                        <div id="side-content" className="d-none d-lg-block push">
                            {
                                [...(communicationStore.turnModel?.players || []), ...(communicationStore.turnModel?.diedPlayers || [])].map((player, index) => (
                                    <GamePlayers key={index} Player={player} Died={communicationStore.turnModel?.diedPlayers.includes(player) ?? false} />
                                ))
                            }
                        </div>
                    </div>
                </div>
                <div className="col-md-8 col-lg-7 col-xl-9">
                    <div className="content content-full">
                        {
                            communicationStore.turnModel?.actionModel &&
                            communicationStore.turnModel?.actionModel?.gameUsers &&
                            communicationStore.turnModel?.actionModel?.gameUsers.length > 0 &&
                            (<GameAction isValid={communicationStore.turnModel?.isActionValid ?? false} actionModel={communicationStore.turnModel.actionModel} />)
                        }
                    </div>
                </div>
            </div>
        </main>
    );
}

export default observer(GamePlan);