import { useState } from "react";
import { PlayerModel } from "../../../models/PlayerModel";
import { Ability } from "./Ability";
import { PlayerTrigger } from "./PlayerTrigger";
import { useStore } from "../../../Store";
import { AppConfig } from "../../../models/AppConfig";

interface ActivePlayerProps {
    player: PlayerModel;
    roomId: string;
}

export function ActivePlayer({ player, roomId }: ActivePlayerProps) {
    const [showAbilities, setShowAbilities] = useState(false);
    const { communicationStore } = useStore();
    const userId = communicationStore.turnModel?.data?.userId ?? '';
    const abilities = communicationStore.turnModel?.data?.abilities || [];

    const handleUseAbility = (abilityName: string) => {
        communicationStore.doAction(
            AppConfig.appId,
            roomId,
            userId,
            abilityName,
            player.userId
        );
    };

    if (abilities.length === 0)
        return <></>

    if (showAbilities) {
        return (
            <Ability
                abilities={abilities}
                onBack={() => setShowAbilities(false)}
                onUseAbility={handleUseAbility}
            />
        );
    }

    return (
        <PlayerTrigger playerName={player.name} onClick={() => setShowAbilities(true)} />
    );
}