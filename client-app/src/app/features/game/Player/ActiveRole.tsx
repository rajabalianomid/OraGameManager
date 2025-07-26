import { useState } from "react";
import { PlayerModel } from "../../../models/PlayerModel";
import { RoleTrigger } from "./RoleTrigger";
import { RolePlayers } from "./RolePlayers";

interface ActiveRoleProps {
    roleName: string;
    players: PlayerModel[];
}

export function ActiveRole({ roleName, players }: ActiveRoleProps) {
    const [showPlayers, setShowPlayers] = useState(false);


    if (showPlayers) {
        return (
            <RolePlayers
                players={players}
                onBack={() => setShowPlayers(false)}
            />
        );
    }

    return (
        <RoleTrigger roleName={roleName} onClick={() => setShowPlayers(true)} />
    );
}
