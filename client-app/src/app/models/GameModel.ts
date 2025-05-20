import { ActionModelResponse } from "./ActionModelResponse";
import { HasMeetingType, TurnStatusType } from "./Enums";
import { GameUserModel } from "./GameUserModel";
import { MessageModel } from "./MessageModel";
import { PlayerModel } from "./PlayerModel";
import { RoleModel } from "./RoleModel ";


export interface GameModel {
    players: PlayerModel[];
    meetingPlayer: PlayerModel[];
    diedPlayers: PlayerModel[];
    playersActionCard: PlayerModel[];
    roles: RoleModel[];
    currentRole: RoleModel;
    actionResponse?: ActionModelResponse;
    challenger: GameUserModel[];
    note: string;
    gameId: number;
    startedGame: boolean;
    turnType: TurnStatusType;
    tempTurnType: TurnStatusType;
    isActionValid: boolean;
    actionSound: string;
    isChallenge: boolean;
    hasMeeting: HasMeetingType;
    meetingId: string;
    userMeetingToken: string;
    messages: MessageModel[];
}