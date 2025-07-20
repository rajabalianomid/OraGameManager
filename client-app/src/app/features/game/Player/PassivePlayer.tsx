import avatar15 from '../../../../assets/media/avatars/avatar15.jpg';
import photo10 from '../../../../assets/media/photos/photo10.jpg';
import { PlayerModel } from '../../../models/PlayerModel';

interface PassivePlayerProps {
    player: PlayerModel;
    died: boolean;
}

export function PassivePlayer({ player, died }: PassivePlayerProps) {
    return (
        <div className="block block-rounded text-center bg-image" style={{ backgroundImage: `url(${photo10})` }}>
            <div className="block-content">
                <div className="js-pie-chart pie-chart js-pie-chart-enabled" data-percent="100" data-line-width="4" data-size="100" data-bar-color="#fff">
                    <span>
                        <img
                            className={`img-avatar img-avatar-thumb ${died ? "img-avatar-thumb-red" : "img-avatar-thumb-green"}`}
                            src={avatar15}
                            alt=""
                        />
                    </span>
                    <canvas height="100" width="100"></canvas>
                </div>
            </div>
            <div className="block-content">
                <p className="text-white text-uppercase fs-sm fw-bold">
                    {player.name.length > 15
                        ? <span title={player.name}>{player.name.slice(0, 15) + '...'}</span>
                        : player.name}
                </p>
            </div>
        </div>
    );
}
