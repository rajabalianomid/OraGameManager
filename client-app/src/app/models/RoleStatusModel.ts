import { AbilityModel } from "./AbilityModel";

export interface RoleStatusModel {
    roleName: string;
    health: number;
    abilityCount: number;
    selfAbilityCount: number;
    hasNightAbility: boolean;
    hasDayAbility: boolean;
    canSpeak: boolean;
    darkSide: boolean;
    abilities?: AbilityModel[];
}