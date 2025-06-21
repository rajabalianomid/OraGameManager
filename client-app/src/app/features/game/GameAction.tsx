import { useEffect, useState } from "react";
import { ActionModel } from "../../models/ActionModel";
import { useStore } from "../../Store";
import { observer } from "mobx-react-lite";

interface GameActionProps {
    isValid: boolean;
    actionModel: ActionModel;
}

function GameAction(props: GameActionProps) {
    const { communicationStore, roomStore } = useStore();
    const [showButtons, setShowButtons] = useState<{ [key: string]: boolean }>({});

    const handleLinkClick = (userId: string) => {
        setShowButtons((prev) => ({ ...prev, [userId]: true }));
    };

    const handleBackClick = (userId: string) => {
        setShowButtons((prev) => ({ ...prev, [userId]: false }));
    };

    useEffect(() => {
        var x = roomStore.getCurrentRoom();
        debugger;
        var roomid = roomStore.currentRoom?.id?.toString();
        debugger;
    }, []);

    return (
        <div className="row">
            {props.actionModel.gameUsers.map((user) => (
                <div className="col-md-6 col-xl-3 col-sm-12 col-xs-12" key={user.userId}>
                    {showButtons[user.userId] ? (
                        <div className="block block-rounded block-link-shadow d-flex justify-content-center align-items-start text-center bg-xinspire h-100 mb-0">
                            <div className="block-content block-content-full bg-body-extra-light mt-1 align-self-stretch">
                                <div className="py-4">
                                    <h4 className="fs-lg fw-semibold mt-3 mb-1">Select an action for {user.name}</h4>
                                    <div className="block-content block-content-full">
                                        <div className="row g-sm">
                                            {props.actionModel.buttons.map((button, index) => (
                                                <div className="col-12" style={{ padding: "10px" }} key={index}>
                                                    <a className={`btn w-100 btn-alt-${button.buttonStyle}`} href="#" onClick={() => roomStore.currentRoom?.id && communicationStore.doAction(roomStore.currentRoom.id, user.userId, button.isGun)} >
                                                        <i className={`fa fa-fw fa-${button.buttonIcon} me-1`}></i> {button.buttonName}
                                                    </a>
                                                </div>
                                            ))}
                                            <div className="col-12" style={{ padding: "10px" }}>
                                                <a className="btn w-100 btn-alt-secondary" href="#" onClick={() => handleBackClick(user.userId)}>
                                                    <i className="fa fa-fw fa-arrow-left me-1"></i> Back
                                                </a>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                    ) : (
                        <a className="block block-rounded block-link-shadow d-flex justify-content-center align-items-start text-center bg-xinspire h-100 mb-0"
                            href="#"
                            onClick={() => handleLinkClick(user.userId)}>
                            <div className="block-content block-content-full bg-body-extra-light mt-1 align-self-stretch">
                                <div className="py-4">
                                    <i className="fa fa-2x fa-user text-xinspire"></i>
                                    <p className="fs-lg fw-semibold mt-3 mb-1">{user.name}</p>
                                    <p className="text-muted mb-0">Click to see actions</p>
                                </div>
                            </div>
                        </a>
                    )}
                </div>
            ))}
        </div>
    );
}

export default observer(GameAction);