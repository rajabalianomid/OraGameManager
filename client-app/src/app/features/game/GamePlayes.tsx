import { observer } from 'mobx-react-lite';
import { useState } from 'react';
import { useStore } from '../../Store';
import { PlayerModel } from '../../models/PlayerModel';
import { SidePlayer } from './Player/SidePlayer';
import { AppConfig } from '../../models/AppConfig';
import { Ability } from './Player/Ability.tsx';
import { PlayerTrigger } from './Player/PlayerTrigger';
import { LastCardChanceModel } from '../../models/LastCardChanceModel.ts';
import { CardItem } from './Player/CardItem.tsx';

interface GamePlayersProps {
    Player?: PlayerModel;
    Died?: boolean;
    RoomId?: string;
    Card?: LastCardChanceModel;
    ActOnMe: boolean;
}

function GamePlayers({ Player, Died, RoomId, Card, ActOnMe }: GamePlayersProps) {
    const { communicationStore } = useStore();
    const [showAbilities, setShowAbilities] = useState(false);
    debugger;
    if (!ActOnMe && Player) {
        return <SidePlayer Player={Player} Died={Died ?? false} />;
    }
    const abilities = communicationStore.turnModel?.data?.abilities || [];
    const userId = communicationStore.turnModel?.data?.userId ?? '';

    const handleUseAbility = (abilityName: string) => {
        if (!Player || !RoomId) return;
        communicationStore.doAction(
            AppConfig.appId,
            RoomId,
            userId,
            abilityName,
            Player.userId
        );
    };

    if (showAbilities) {
        return <Ability abilities={abilities} onBack={() => setShowAbilities(false)} onUseAbility={handleUseAbility} />;
    }

    if (abilities.length > 0 && Player) {
        return <PlayerTrigger playerName={Player.name} onClick={() => setShowAbilities(true)} />;
    }

    if (Card != null)
        return <CardItem card={Card} />;

    return null;
}

export default observer(GamePlayers);
