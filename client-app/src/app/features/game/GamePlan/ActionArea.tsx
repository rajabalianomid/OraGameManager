import { PlayerModel } from "../../../models/PlayerModel";
import { LastCardChanceModel } from "../../../models/LastCardChanceModel";
import { ActivePlayer } from "../Player/ActivePlayer";
import { ActiveCard } from "../Player/ActiveCard";

interface ActionAreaProps {
    players: PlayerModel[];
    cards: LastCardChanceModel[];
    roomId: string;
}

export default function ActionArea({ players, cards, roomId }: ActionAreaProps) {

    debugger;
    return (
        <>
            <div className="row g-sm">
                {players.map((player, i) => (
                    <div key={`player-${i}`} className="col-md-2 col-lg-2 col-xl-2">
                        <ActivePlayer player={player} roomId={roomId} />
                    </div>
                ))}
            </div>
            <ActiveCard players={players} cards={cards} roomId={roomId} />
        </>
    );
}
