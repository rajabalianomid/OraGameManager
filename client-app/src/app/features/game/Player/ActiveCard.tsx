import { useStore } from "../../../Store";
import { AppConfig } from "../../../models/AppConfig";
import { LastCardChanceModel } from "../../../models/LastCardChanceModel";
import { PlayerModel } from "../../../models/PlayerModel";
import { CardItem } from "./CardItem";
import { PlayerTrigger } from "./PlayerTrigger";

interface ActiveCardProps {
    players: PlayerModel[];
    cards: LastCardChanceModel[];
    roomId: string;
}

export function ActiveCard({ cards, players, roomId }: ActiveCardProps) {
    // const [showAbilities, setShowAbilities] = useState(false);
    const { communicationStore } = useStore();
    const userId = communicationStore.turnModel?.data?.userId ?? '';
    // const abilities = communicationStore.turnModel?.data?.abilities || [];

    const handleSelectCard = (cardName: string) => {
        debugger;
        communicationStore.doAction(
            AppConfig.appId,
            roomId,
            userId,
            cardName,
            ""
        );
    }
    const handleUseCardOnPlayer = (cardName: string, playerName: string) => {
        debugger;
        communicationStore.commitAction(
            AppConfig.appId,
            roomId,
            userId,
            cardName,
            playerName
        );
    };
    if (players.length > 0 && cards.length === 1 && cards[0].showFront) {
        return (
            <div className="row g-sm">
                {players.map((player, i) => (
                    <div key={`player-${i}`} className="col-md-2 col-lg-2 col-xl-2">
                        <PlayerTrigger playerName={player.name} onClick={() => handleUseCardOnPlayer(cards[0].name, player.name)} />
                    </div>
                ))}
            </div>
        );
    }
    console.log("cards", cards);
    return (
        <div>
            <div className="row g-sm">
                {
                    cards.map((card, i) => (
                        <div key={`card-${i}`} className="col-md-2 col-lg-2 col-xl-2">
                            <CardItem card={card} onClick={() => handleSelectCard(card.name)} />
                        </div>
                    ))
                }
            </div>
        </div>
    );
}