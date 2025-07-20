import { PlayerModel } from "../../../models/PlayerModel";
import { PassivePlayer } from "../Player/PassivePlayer";

interface PlayerListProps {
    alive: PlayerModel[];
    dead: PlayerModel[];
    roomId: string;
}

export default function PlayerList({ alive, dead }: PlayerListProps) {
    return (
        <>
            {alive.map((player, i) => (
                <PassivePlayer key={`alive-${i}`} player={player} died={false} />
            ))}
            {dead.map((player, i) => (
                <PassivePlayer key={`dead-${i}`} player={player} died={true} />
            ))}
        </>
    );
}
