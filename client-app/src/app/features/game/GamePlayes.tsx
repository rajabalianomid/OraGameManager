import { observer } from 'mobx-react-lite';
import avatar15 from '../../../assets/media/avatars/avatar15.jpg';
import photo10 from '../../../assets/media/photos/photo10.jpg';
import { PlayerModel } from '../../models/PlayerModel';
import { useStore } from '../../Store';
import { AppConfig } from '../../models/AppConfig';

interface GamePlayersProps {
    Player: PlayerModel;
    Died: boolean;
}

function GamePlayers(props: GamePlayersProps) {

    const { communicationStore, mainStore, profileStore } = useStore();

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
                    {props.Player.name}
                </p>
                <div className="fs-sm text-muted mb-0">
                    <div className="btn-toolbar mb-2 btn-toolbar-center" role="toolbar" aria-label="Icons Toolbar with button groups">
                        <div className="btn-group me-2 mb-2" role="group" aria-label="Icons Text group">
                            {
                                (communicationStore.turnModel?.data?.abilities || []).map((ability, index) => (
                                    <button key={index} type="button" className="btn btn-primary">
                                        {/* <i className={`fa fa-fw fa-${ability.icon}`}></i> */}
                                        <i className="fa fa-fw fa-bold" title={`${ability}`} onClick={() => communicationStore.doAction(AppConfig.appId, communicationStore.turnModel?.data?.userId ?? '')}></i>
                                    </button>
                                ))
                            }
                        </div>
                    </div>
                </div>
            </div>
        </div >
    );
}

export default observer(GamePlayers);