import { ButtonDetail } from "./ButtonDetail";
import { CardModel } from "./CardModel";
import { GameUserModel } from "./GameUserModel";

export interface ActionModel {
    name: string;
    buttons: ButtonDetail[];
    description: string;
    gameUsers: GameUserModel[];
    cards: CardModel[];
}