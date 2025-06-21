import { ExtraPlayerInfo } from "./ExtraPlayerInfo";

export interface ExtraInfoDetailsModel {
    forceNextTurns: string[];
    extraPlayerInfo?: ExtraPlayerInfo[];
    isYourTurn: boolean;
    reminderTime: number;
}