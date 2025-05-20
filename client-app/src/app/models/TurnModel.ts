import { ActionModel } from "./ActionModel ";
import { HasMeetingType, TurnStatusType } from "./Enums";
import { PlayerModel } from "./PlayerModel";

export interface TurnModel {
    turnStatusType: TurnStatusType;
    gameTurnStatusType: TurnStatusType;
    isActionValid: boolean;
    isChallenge: boolean;
    hasMeeting: HasMeetingType;
    token: string;
    meetingRoomId: string;
    players: PlayerModel[];
    meetingPlayers: PlayerModel[];
    diedPlayers: PlayerModel[];
    playersActionCard: PlayerModel[];
    actionModel: ActionModel;
}