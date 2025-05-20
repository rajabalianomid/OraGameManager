import { ActionModel } from "./ActionModel ";
import { TurnStatusType } from "./Enums";

export interface RoleModel {
    id: number;
    name: string;
    userName: string;
    userId: string;
    currentTurn: TurnStatusType;
    doAction: ActionModel;
    selectedGameUserId: string;
}

