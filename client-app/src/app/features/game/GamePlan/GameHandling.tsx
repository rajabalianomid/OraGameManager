// import { observer } from "mobx-react-lite";
// import { PlayerModel } from "../../../models/PlayerModel";
// import { LastCardChanceModel } from "../../../models/LastCardChanceModel";
// import { CardItem } from "../Player/CardItem";
// import { ActivePlayer } from "../Player/ActivePlayer";
// import { PassivePlayer } from "../Player/PassivePlayer";

// interface GamePlayersProps {
//     Player?: PlayerModel;
//     Died?: boolean;
//     RoomId?: string;
//     Card?: LastCardChanceModel;
//     ActOnMe: boolean;
// }

// function GameHandling({ Player, Died = false, RoomId = "", Card, ActOnMe }: GamePlayersProps) {
//     if (Card) {
//         return <CardItem card={Card} />;
//     }

//     if (ActOnMe && Player) {
//         return <ActivePlayer player={Player} roomId={RoomId} />;
//     }

//     if (Player) {
//         return <PassivePlayer player={Player} died={Died} />;
//     }

//     return null;
// }

// export default observer(GameHandling);