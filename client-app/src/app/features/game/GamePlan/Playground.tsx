// Playground.tsx
import { useState } from "react";
import { useParams } from "react-router-dom";
import { observer } from "mobx-react-lite";
import { useStore } from "../../../Store";
import VideoCall from "../../voip/VideoCall";
import { useGameConnection } from "./Hooks/useGameConnection";
import PlayerList from "./PlayerList";
import ActionArea from "./ActionArea";

function Playground() {
    const { roomId } = useParams<{ roomId: string }>();
    const { communicationStore } = useStore();
    const [playersCloseBox, setPlayersCloseBox] = useState(false);
    const [playersFullScreen, setPlayersFullScreen] = useState(false);

    useGameConnection(roomId);

    const turnData = communicationStore.turnModel?.data;

    return (
        <main id="main-container">
            <div className="row g-0 flex-md-grow-1">
                {/* Sidebar: Player List */}
                <div className="col-md-3 col-lg-4 col-xl-2 bg-body-dark h100-scroll">
                    <div className="content">
                        <div className="d-lg push">
                            <div className={`block block-rounded ${playersCloseBox ? "block-mode-hidden" : ""} ${playersFullScreen ? "block-mode-fullscreen" : ""}`}>
                                <div className="block-header block-header-default block-header-rtl">
                                    <h3 className="block-title">Players</h3>
                                    <div className="block-options">
                                        <button
                                            type="button"
                                            className="btn-block-option"
                                            onClick={() => setPlayersCloseBox(!playersCloseBox)}
                                        >
                                            <i className="si si-arrow-down"></i>
                                        </button>
                                        <button
                                            type="button"
                                            className="btn-block-option"
                                            onClick={() => setPlayersFullScreen(!playersFullScreen)}
                                        >
                                            <i className="si si-size-fullscreen"></i>
                                        </button>
                                    </div>
                                </div>
                                <div className="block-content">
                                    <PlayerList
                                        alive={turnData?.alivePlayers || []}
                                        dead={turnData?.deadPlayers || []}
                                        roomId={roomId ?? ""}
                                    />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                {/* Main Content */}
                <div className="col-md-9 col-lg-8 col-xl-10">
                    <div className="content">
                        <ActionArea
                            players={turnData?.actingOn || []}
                            cards={turnData?.cards || []}
                            roomId={roomId ?? ""}
                        />
                    </div>
                    <VideoCall />
                </div>
            </div>
        </main>
    );
}

export default observer(Playground);
