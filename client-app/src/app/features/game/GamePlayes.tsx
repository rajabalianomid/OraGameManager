import { observer } from 'mobx-react-lite';
import avatar15 from '../../../assets/media/avatars/avatar15.jpg';
import photo10 from '../../../assets/media/photos/photo10.jpg';
import { PlayerModel } from '../../models/PlayerModel';
import { useStore } from '../../Store';
import { AppConfig } from '../../models/AppConfig';
import { useState } from 'react';

interface GamePlayersProps {
    Player: PlayerModel;
    Died: boolean;
    RoomId: string;
    ActOnMe: boolean;
}

function GamePlayers(props: GamePlayersProps) {
    const { communicationStore, mainStore, profileStore } = useStore();
    const [showAbilities, setShowAbilities] = useState(false);
    console.log("Ability", communicationStore.turnModel?.data);

    if (!props.ActOnMe) {
        return (
            <div className="block block-rounded text-center bg-image" style={{ backgroundImage: `url(${photo10})` }}>
                <div className="block-content">
                    <div className="js-pie-chart pie-chart js-pie-chart-enabled" data-percent="100" data-line-width="4" data-size="100" data-bar-color="#fff">
                        <span>
                            <img className={`img-avatar img-avatar-thumb ${props.Died ? "img-avatar-thumb-red" : "img-avatar-thumb-green"} `} src={avatar15} alt="" />
                        </span>
                        <canvas height="100" width="100"></canvas></div>
                </div>
                <div className="block-content">
                    <p className="text-white text-uppercase fs-sm fw-bold">
                        {props.Player.name.length > 15
                            ? (
                                <span title={props.Player.name}>
                                    {props.Player.name.slice(0, 15) + '...'}
                                </span>
                            )
                            : props.Player.name}
                    </p>
                </div>
            </div >
        );
    }
    else {
        if (showAbilities) {
            // Render abilities tiles
            return (
                communicationStore.turnModel?.data?.abilities || []).map((ability, idx) => (
                    <div>
                        <a className="block text-center bg-primary" onClick={() =>
                            communicationStore.doAction(
                                AppConfig.appId,
                                props.RoomId,
                                communicationStore.turnModel?.data?.userId ?? '', ability.name, props.Player.userId
                            )}>
                            <div className="block-content block-content-full ratio ratio-16x9">
                                <div className="d-flex justify-content-center align-items-center">
                                    <div>
                                        <i className={`far fa-2x fa-${ability.icon} text-primary-lighter`}></i>
                                        <div className="fw-semibold mt-3 text-uppercase text-white">{ability.name}</div>
                                    </div>
                                </div>
                            </div>
                        </a>
                        <a className="block text-center bg-xmodern" href="#" onClick={() => setShowAbilities(false)}>
                            <div className="block-content block-content-full ratio">
                                <div className="d-flex justify-content-center align-items-center">
                                    <div>
                                        <i className="fa fa-2x fa-arrow-left text-xmodern-lighter"></i>
                                    </div>
                                </div>
                            </div>
                        </a>
                    </div>

                ));
        }

        if (
            communicationStore.turnModel?.data?.abilities &&
            communicationStore.turnModel.data.abilities.length > 0
        ) {
            return (
                <a
                    href="#"
                    className="block block-link-pop text-center"
                    onClick={e => {
                        e.preventDefault();
                        setShowAbilities(true);
                    }}
                >
                    <div className="block block-link-pop bg-xpro text-white h-100 mb-0">
                        <div className="block-content text-center py-5">
                            <p className="mb-4">
                                <i className="fa fa-user-check fa-3x"></i>
                            </p>
                            <p className="fs-4 fw-bold mb-0">
                                {props.Player.name.length > 15
                                    ? (
                                        <span title={props.Player.name}>
                                            {props.Player.name.slice(0, 15) + '...'}
                                        </span>
                                    )
                                    : props.Player.name}
                            </p>
                        </div>
                    </div>
                </a>
            );
        } else {
            return null; // or fallback content if needed
        }

    }

}

export default observer(GamePlayers);