import { PlayerModel } from "./PlayerModel";
import { RoleStatusModel } from "./RoleStatusModel";

export interface TurnModel {
    phase?: string;
    round: number;
    canSpeak: boolean;
    remindTime: number;
    abilities?: string[];
    roleStatus?: RoleStatusModel;
    alivePlayers: PlayerModel[];
    deadPlayers: PlayerModel[];
}