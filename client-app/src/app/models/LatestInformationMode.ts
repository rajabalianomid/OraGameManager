import { AbilityModel } from "./AbilityModel";
import { LastCardChanceModel } from "./LastCardChanceModel";
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
    cards: LastCardChanceModel[];
    actingOn: PlayerModel[];
    hasVideo: boolean;
}