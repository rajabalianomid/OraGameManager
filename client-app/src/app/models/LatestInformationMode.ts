import { PlayerModel } from "./PlayerModel";
import { RoleStatusModel } from "./RoleStatusModel";

export interface LatestInformationModel {
    userId: string;
    phase?: string;
    round: number;
    roleStatus?: RoleStatusModel;
    isAlive: boolean;
    abilities?: string[];
    alivePlayers: PlayerModel[];
    deadPlayers: PlayerModel[];
}