import { PlayerModel } from "../../../models/PlayerModel";

interface RolePlayersProps {
    players: PlayerModel[];
    onBack: () => void;
}

export function RolePlayers({ players, onBack }: RolePlayersProps) {
    return (
        <>
            {players.map((player, idx) => (
                <div key={idx}>
                    <a className="block text-center bg-primary">
                        <div className="block-content block-content-full ratio ratio-16x9">
                            <div className="d-flex justify-content-center align-items-center">
                                <div>
                                    <i className="far fa-2x fa-user text-primary-lighter"></i>
                                    <div className="fw-semibold mt-3 text-uppercase text-white">{player.name}</div>
                                </div>
                            </div>
                        </div>
                    </a>
                </div>
            ))}
            <a className="block text-center bg-xmodern" href="#" onClick={onBack}>
                <div className="block-content block-content-full ratio">
                    <div className="d-flex justify-content-center align-items-center">
                        <div>
                            <i className="fa fa-2x fa-arrow-left text-xmodern-lighter"></i>
                        </div>
                    </div>
                </div>
            </a>
        </>
    );
}