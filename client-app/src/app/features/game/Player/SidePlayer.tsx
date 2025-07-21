// import avatar15 from '../../../../assets/media/avatars/avatar15.jpg';
// import photo10 from '../../../../assets/media/photos/photo10.jpg';
// import { PlayerModel } from '../../../models/PlayerModel';

// interface PlayerCardProps {
//     Player: PlayerModel;
//     Died: boolean;
// }

// export function SidePlayer({ Player, Died }: PlayerCardProps) {
//     return (
//         <div className="block block-rounded text-center bg-image" style={{ backgroundImage: `url(${photo10})` }}>
//             <div className="block-content">
//                 <div className="js-pie-chart pie-chart js-pie-chart-enabled" data-percent="100" data-line-width="4" data-size="100" data-bar-color="#fff">
//                     <span>
//                         <img
//                             className={`img-avatar img-avatar-thumb ${Died ? "img-avatar-thumb-red" : "img-avatar-thumb-green"}`}
//                             src={avatar15}
//                             alt=""
//                         />
//                     </span>
//                     <canvas height="100" width="100"></canvas>
//                 </div>
//             </div>
//             <div className="block-content">
//                 <p className="text-white text-uppercase fs-sm fw-bold">
//                     {Player.name.length > 15
//                         ? <span title={Player.name}>{Player.name.slice(0, 15) + '...'}</span>
//                         : Player.name}
//                 </p>
//             </div>
//         </div>
//     );
// }
