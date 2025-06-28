import { AbilityModel } from "./AbilityModel";
import { PlayerModel } from "./PlayerModel";
import { RoleStatusModel } from "./RoleStatusModel";

export interface LatestInformationModel {
    userId: string;
    phase?: string;
    round: number;
    roleStatus?: RoleStatusModel;
    isAlive: boolean;
    abilities: AbilityModel[];
    alivePlayers: PlayerModel[];
    deadPlayers: PlayerModel[];
    hasVideo: boolean;
}