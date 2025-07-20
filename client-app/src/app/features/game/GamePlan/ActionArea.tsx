import { PlayerModel } from "../../../models/PlayerModel";
import { LastCardChanceModel } from "../../../models/LastCardChanceModel";
import { CardItem } from "../Player/CardItem";
import { ActivePlayer } from "../Player/ActivePlayer";

interface ActionAreaProps {
    players: PlayerModel[];
    cards: LastCardChanceModel[];
    roomId: string;
}

export default function ActionArea({ players, cards, roomId }: ActionAreaProps) {
    return (
        <div className="row g-sm">
            {players.map((player, i) => (
                <div key={`player-${i}`} className="col-md-2 col-lg-2 col-xl-2">
                    <ActivePlayer player={player} roomId={roomId} />
                </div>
            ))}
            {cards.map((card, i) => (
                <div key={`card-${i}`} className="col-md-2 col-lg-2 col-xl-2">
                    <CardItem card={card} />
                </div>
            ))}
        </div>
    );
}
